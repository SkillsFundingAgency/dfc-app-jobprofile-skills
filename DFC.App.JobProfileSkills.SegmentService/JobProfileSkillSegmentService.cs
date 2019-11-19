using AutoMapper;
using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.SegmentService
{
    public class JobProfileSkillSegmentService : IJobProfileSkillSegmentService
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public JobProfileSkillSegmentService(ICosmosRepository<JobProfileSkillSegmentModel> repository, IMapper mapper, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobProfileSkillSegmentModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<JobProfileSkillSegmentModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<JobProfileSkillSegmentModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName.ToLower() == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> UpsertAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            if (skillSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(skillSegmentModel));
            }

            if (skillSegmentModel.Data == null)
            {
                skillSegmentModel.Data = new JobProfileSkillSegmentDataModel();
            }

            return await UpsertAndRefreshSegmentModel(skillSegmentModel).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);
            return result == HttpStatusCode.NoContent;
        }

        public async Task<HttpStatusCode> PatchDigitalSkillAsync(PatchDigitalSkillModel patchModel, Guid documentId)
        {
            if (patchModel == null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);

            if (existingSegmentModel == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            existingSegmentModel.Data.DigitalSkill = patchModel.MessageAction == MessageAction.Deleted ? string.Empty : patchModel.Title;
            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchOnetSkillAsync(PatchOnetSkillModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingSkillMatrix = existingSegmentModel.Data.Skills.SingleOrDefault(s => s.OnetSkill.Id == patchModel.Id);
            if (existingSkillMatrix is null)
            {
                return patchModel.MessageAction == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            if (patchModel.MessageAction == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Skills.SingleOrDefault(s => s.OnetSkill.Id == patchModel.Id).OnetSkill = new OnetSkill();
            }
            else
            {
                var updatedOnetSkill = mapper.Map<OnetSkill>(patchModel);
                existingSegmentModel.Data.Skills.SingleOrDefault(s => s.OnetSkill.Id == patchModel.Id).OnetSkill = updatedOnetSkill;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchSkillsMatrixAsync(PatchContextualisedModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingSkillMatrix = existingSegmentModel.Data.Skills.SingleOrDefault(sm => sm.ContextualisedSkill.Id == patchModel.Id);
            if (existingSkillMatrix is null)
            {
                return patchModel.MessageAction == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            var existingIndex = existingSegmentModel.Data.Skills.ToList().FindIndex(ai => ai.ContextualisedSkill.Id == patchModel.Id);
            if (patchModel.MessageAction == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Skills.RemoveAt(existingIndex);
            }
            else
            {
                var updatedSkillsMatrix = mapper.Map<Skills>(patchModel);
                existingSegmentModel.Data.Skills[existingIndex] = updatedSkillsMatrix;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchRestrictionAsync(PatchRestrictionModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingSkillMatrix = existingSegmentModel.Data.Restrictions.SingleOrDefault(sm => sm.Id == patchModel.Id);
            if (existingSkillMatrix is null)
            {
                return patchModel.MessageAction == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            var existingIndex = existingSegmentModel.Data.Restrictions.ToList().FindIndex(ai => ai.Id == patchModel.Id);
            if (patchModel.MessageAction == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Restrictions.RemoveAt(existingIndex);
            }
            else
            {
                var updatedRestriction = mapper.Map<Data.Models.Restriction>(patchModel);
                existingSegmentModel.Data.Restrictions[existingIndex] = updatedRestriction;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        private async Task<HttpStatusCode> UpsertAndRefreshSegmentModel(JobProfileSkillSegmentModel existingSegmentModel)
        {
            var result = await repository.UpsertAsync(existingSegmentModel).ConfigureAwait(false);

            if (result == HttpStatusCode.OK || result == HttpStatusCode.Created)
            {
                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(existingSegmentModel);

                await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            }

            return result;
        }
    }
}