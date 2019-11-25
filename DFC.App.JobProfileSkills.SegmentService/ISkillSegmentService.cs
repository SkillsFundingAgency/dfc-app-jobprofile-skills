using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Data.Contracts
{
    public interface ISkillSegmentService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<JobProfileSkillSegmentModel>> GetAllAsync();

        Task<JobProfileSkillSegmentModel> GetByIdAsync(Guid documentId);

        Task<JobProfileSkillSegmentModel> GetByNameAsync(string canonicalName);

        Task<HttpStatusCode> UpsertAsync(JobProfileSkillSegmentModel skillSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);

        Task<HttpStatusCode> PatchDigitalSkillAsync(PatchDigitalSkillModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchOnetSkillAsync(PatchOnetSkillModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchSkillsMatrixAsync(PatchContextualisedModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchRestrictionAsync(PatchRestrictionModel patchModel, Guid documentId);
    }
}