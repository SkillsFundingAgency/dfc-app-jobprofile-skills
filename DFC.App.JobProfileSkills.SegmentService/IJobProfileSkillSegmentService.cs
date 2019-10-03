using DFC.App.JobProfileSkills.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Data.Contracts
{
    public interface IJobProfileSkillSegmentService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<JobProfileSkillSegmentModel>> GetAllAsync();

        Task<JobProfileSkillSegmentModel> GetByIdAsync(Guid documentId);

        Task<JobProfileSkillSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<UpsertJobProfileSkillsModelResponse> UpsertAsync(JobProfileSkillSegmentModel skillSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}