using RestSharp;
using System;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory.Interface
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUrl);
    }
}
