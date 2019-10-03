using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.SegmentService
{
    public class JobProfileSkillSegmentService : IJobProfileSkillSegmentService
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;

        public JobProfileSkillSegmentService(ICosmosRepository<JobProfileSkillSegmentModel> repository)
        {
            this.repository = repository;
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

        public async Task<JobProfileSkillSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName.ToLower() == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<UpsertJobProfileSkillsModelResponse> UpsertAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            if (skillSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(skillSegmentModel));
            }

            if (skillSegmentModel.Data == null)
            {
                skillSegmentModel.Data = new JobProfileSkillSegmentDataModel();
            }

            var result = await repository.UpsertAsync(skillSegmentModel).ConfigureAwait(false);

            return new UpsertJobProfileSkillsModelResponse
            {
                JobProfileSkillSegmentModel = skillSegmentModel,
                ResponseStatusCode = result,
            };
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);
            return result == HttpStatusCode.NoContent;
        }
    }
}