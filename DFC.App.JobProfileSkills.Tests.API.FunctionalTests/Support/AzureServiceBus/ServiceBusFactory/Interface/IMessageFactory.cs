using Microsoft.Azure.ServiceBus;

namespace DFC.App.CareerPath.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface IMessageFactory
    {
        Message Create(string messageId, byte[] messageBody, string actionType, string contentType);
    }
}
