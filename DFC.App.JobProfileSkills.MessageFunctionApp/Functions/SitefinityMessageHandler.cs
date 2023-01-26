using Azure.Messaging.ServiceBus;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
//using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private static readonly string ClassFullName = typeof(SitefinityMessageHandler).FullName;
        private readonly IMessageProcessor messageProcessor;
        private readonly IMessagePropertiesService messagePropertiesService;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public SitefinityMessageHandler(
            IMessageProcessor messageProcessor,
            IMessagePropertiesService messagePropertiesService,
            ILogService logService,
            ICorrelationIdProvider correlationIdProvider)
        {
            this.messageProcessor = messageProcessor;
            this.messagePropertiesService = messagePropertiesService;
            this.logService = logService;
            this.correlationIdProvider = correlationIdProvider;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] ServiceBusMessage sitefinityMessage)
        {
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            correlationIdProvider.CorrelationId = sitefinityMessage.CorrelationId;

            sitefinityMessage.ApplicationProperties.TryGetValue("ActionType", out var actionType);
            sitefinityMessage.ApplicationProperties.TryGetValue("CType", out var contentType);
            sitefinityMessage.ApplicationProperties.TryGetValue("Id", out var messageContentId);

            logService.LogInformation($"{nameof(SitefinityMessageHandler)}: Received message action '{actionType}' for type '{contentType}' with Id: '{messageContentId}'");

            var message = Encoding.UTF8.GetString(sitefinityMessage?.Body);

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
            }

            if (!Enum.IsDefined(typeof(MessageAction), actionType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(actionType), $"Invalid message action '{actionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }

            if (!Enum.IsDefined(typeof(MessageContentType), contentType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(contentType), $"Invalid message content type '{contentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var messageAction = Enum.Parse<MessageAction>(actionType?.ToString());
            var messageContentType = Enum.Parse<MessageContentType>(contentType?.ToString());

            var sequenceNumber = messagePropertiesService.GetSequenceNumber(sitefinityMessage);

            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, messageAction).ConfigureAwait(false);

            switch (result)
            {
                case HttpStatusCode.OK:
                    logService.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Updated segment");
                    break;

                case HttpStatusCode.Created:
                    logService.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Created segment");
                    break;

                case HttpStatusCode.AlreadyReported:
                    logService.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment previously updated");
                    break;

                default:
                    logService.LogWarning($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment not Posted: Status: {result}");
                    break;
            }
        }
    }
}