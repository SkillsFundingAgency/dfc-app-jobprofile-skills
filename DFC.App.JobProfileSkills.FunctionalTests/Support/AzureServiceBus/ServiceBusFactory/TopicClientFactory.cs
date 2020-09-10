using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory
{
    public class TopicClientFactory : ITopicClientFactory
    {
        public ITopicClient Create(string connectionString)
        {
            return new TopicClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}
