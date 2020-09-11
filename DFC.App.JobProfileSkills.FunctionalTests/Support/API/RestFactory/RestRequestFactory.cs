using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory
{
    internal class RestRequestFactory : IRestRequestFactory
    {
        public IRestRequest Create(string urlSuffix)
        {
            return new RestRequest(urlSuffix);
        }
    }
}