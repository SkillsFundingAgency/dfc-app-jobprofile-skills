using System.Net.Http;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}