using Azure.Messaging.ServiceBus;
using DFC.Logger.AppInsights.Contracts;
//using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private const int BatchSize = 500;
        private readonly ServiceBusClient topicClient;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly ServiceBusSender sender;

        public JobProfileSegmentRefreshService(ServiceBusClient topicClient, ICorrelationIdProvider correlationIdProvider)
        {
            this.topicClient = topicClient;
            this.correlationIdProvider = correlationIdProvider;
            this.sender = topicClient.CreateSender("SegmentRefresh");
        }

        public async Task SendMessageAsync(TModel model)
        {
            var message = CreateMessage(model);
            await sender.SendMessageAsync(message).ConfigureAwait(false);
        }

        public async Task SendMessageListAsync(IList<TModel> models)
        {
            // List is batched to avoid exceeding the Service Bus size limit on DEV and SIT of 256KB
            if (models != null)
            {
                var listOfMessages = new List<ServiceBusMessage>();
                listOfMessages.AddRange(models.Select(CreateMessage));
                for (var i = 0; i < listOfMessages.Count; i += BatchSize)
                {
                    var batchedList = listOfMessages.Skip(i).Take(BatchSize).ToList();
                    await sender.SendMessagesAsync(batchedList).ConfigureAwait(false);
                }
            }
        }

        private ServiceBusMessage CreateMessage(TModel model)
        {
            var messageJson = JsonConvert.SerializeObject(model);
            return new ServiceBusMessage(Encoding.UTF8.GetBytes(messageJson))
            {
                CorrelationId = correlationIdProvider.CorrelationId,
            };
        }
    }
}