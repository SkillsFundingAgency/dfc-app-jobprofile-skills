using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public interface IHttpClientService
    {
        Task<HttpStatusCode> PostAsync(JobProfileSkillSegmentModel skillSegmentModel);

        Task<HttpStatusCode> PutAsync(JobProfileSkillSegmentModel skillSegmentModel);

        Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel;

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}