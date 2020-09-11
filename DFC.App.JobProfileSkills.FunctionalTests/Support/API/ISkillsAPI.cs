using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.API;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API
{
    public interface ISkillsAPI
    {
        Task<IRestResponse<SkillsAPIResponse>> GetById(string id);
    }
}
