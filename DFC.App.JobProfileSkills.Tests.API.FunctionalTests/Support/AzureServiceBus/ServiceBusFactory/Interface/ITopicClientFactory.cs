using Microsoft.Azure.ServiceBus;

namespace DFC.App.CareerPath.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString);
    }
}
