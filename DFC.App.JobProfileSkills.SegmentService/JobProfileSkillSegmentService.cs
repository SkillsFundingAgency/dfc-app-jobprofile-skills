using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
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

        public async Task<JobProfileSkillSegmentModel> CreateAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            if (skillSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(skillSegmentModel));
            }

            if (skillSegmentModel.Data == null)
            {
                skillSegmentModel.Data = new JobProfileSkillSegmentDataModel();
            }

            var result = await repository.CreateAsync(skillSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.Created
                ? await GetByIdAsync(skillSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<JobProfileSkillSegmentModel> ReplaceAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            if (skillSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(skillSegmentModel));
            }

            if (skillSegmentModel.Data == null)
            {
                skillSegmentModel.Data = new JobProfileSkillSegmentDataModel();
            }

            var result = await repository.UpdateAsync(skillSegmentModel.DocumentId, skillSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.OK
                ? await GetByIdAsync(skillSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<bool> DeleteAsync(Guid documentId, int partitionKeyValue)
        {
            var result = await repository.DeleteAsync(documentId, partitionKeyValue).ConfigureAwait(false);
            return result == HttpStatusCode.NoContent;
        }
    }
}
