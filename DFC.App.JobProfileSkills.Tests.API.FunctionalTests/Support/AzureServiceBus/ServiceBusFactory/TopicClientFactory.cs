using DFC.App.CareerPath.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;

namespace DFC.App.CareerPath.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory
{
    public class TopicClientFactory : ITopicClientFactory
    {
        public ITopicClient Create(string connectionString)
        {
            return new TopicClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}
