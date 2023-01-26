using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public class MessagePropertiesService : IMessagePropertiesService
    {
        public long GetSequenceNumber(ServiceBusMessage message)
        {
            return (message?.;
        }
    }
}