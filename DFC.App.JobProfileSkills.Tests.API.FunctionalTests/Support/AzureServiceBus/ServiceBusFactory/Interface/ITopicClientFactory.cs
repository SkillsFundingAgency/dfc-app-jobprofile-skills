using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString);
    }
}
