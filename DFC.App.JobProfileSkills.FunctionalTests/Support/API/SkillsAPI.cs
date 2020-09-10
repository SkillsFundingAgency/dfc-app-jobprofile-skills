using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.API;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.Support;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API
{
    public class SkillsAPI : ISkillsAPI
    {
        private IRestClientFactory restClientFactory;
        private IRestRequestFactory restRequestFactory;
        private AppSettings appSettings;

        public SkillsAPI(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, AppSettings appSettings)
        {
            this.restClientFactory = restClientFactory;
            this.restRequestFactory = restRequestFactory;
            this.appSettings = appSettings;
        }

        public async Task<IRestResponse<SkillsAPIResponse>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var restClient = this.restClientFactory.Create(this.appSettings.APIConfig.EndpointBaseUrl);
            var restRequest = this.restRequestFactory.Create($"/segment/{id}/contents");
            restRequest.AddHeader("Accept", "application/json");
            return await Task.Run(() => restClient.Execute<SkillsAPIResponse>(restRequest)).ConfigureAwait(false);
        }
    }
}
