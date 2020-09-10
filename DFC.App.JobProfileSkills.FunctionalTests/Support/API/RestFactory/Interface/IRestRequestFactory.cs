using RestSharp;
using System;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory.Interface
{
    public interface IRestRequestFactory
    {
        IRestRequest Create(string urlSuffix);
    }
}
