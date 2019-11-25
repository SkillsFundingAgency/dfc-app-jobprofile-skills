using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}