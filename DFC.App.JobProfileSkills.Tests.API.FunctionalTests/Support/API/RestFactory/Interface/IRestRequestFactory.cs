using RestSharp;
using System;

namespace DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory.Interface
{
    public interface IRestRequestFactory
    {
        IRestRequest Create(string urlSuffix);
    }
}
