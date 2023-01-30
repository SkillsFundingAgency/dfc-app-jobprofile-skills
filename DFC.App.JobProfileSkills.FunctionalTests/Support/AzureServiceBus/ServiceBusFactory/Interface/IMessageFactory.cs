using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface IMessageFactory
    {
        Message Create(string messageId, byte[] messageBody, string actionType, string contentType);
    }
}
