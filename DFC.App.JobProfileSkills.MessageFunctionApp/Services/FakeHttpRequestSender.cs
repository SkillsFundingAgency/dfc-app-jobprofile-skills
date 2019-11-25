using System;
using System.Net.Http;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public class FakeHttpRequestSender : IFakeHttpRequestSender
    {
        public HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException("Now we can setup this method with our mocking framework");
        }
    }
}