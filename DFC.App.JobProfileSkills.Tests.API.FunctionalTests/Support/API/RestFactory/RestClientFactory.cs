using DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;
using System;

namespace DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory
{
    internal class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            return new RestClient(baseUrl);
        }
    }
}
