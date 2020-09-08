using DFC.App.CareerPath.FunctionalTests.Model.API;
using DFC.App.CareerPath.FunctionalTests.Model.Support;
using DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.CareerPath.FunctionalTests.Support.API
{
    public class CareerPathAPI : ICareerPathAPI
    {
        private IRestClientFactory restClientFactory;
        private IRestRequestFactory restRequestFactory;
        private AppSettings appSettings;

        public CareerPathAPI(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, AppSettings appSettings)
        {
            this.restClientFactory = restClientFactory;
            this.restRequestFactory = restRequestFactory;
            this.appSettings = appSettings;
        }

        public async Task<IRestResponse<CareerPathAPIResponse>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var restClient = this.restClientFactory.Create(this.appSettings.APIConfig.EndpointBaseUrl);
            var restRequest = this.restRequestFactory.Create($"/segment/{id}/contents");
            restRequest.AddHeader("Accept", "application/json");
            return await Task.Run(() => restClient.Execute<CareerPathAPIResponse>(restRequest)).ConfigureAwait(false);
        }
    }
}
