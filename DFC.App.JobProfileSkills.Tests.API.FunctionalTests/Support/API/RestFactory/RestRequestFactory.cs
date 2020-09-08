using DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;

namespace DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory
{
    internal class RestRequestFactory : IRestRequestFactory
    {
        public IRestRequest Create(string urlSuffix)
        {
            return new RestRequest(urlSuffix);
        }
    }
}