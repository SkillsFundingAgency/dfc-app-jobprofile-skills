using AutoMapper;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMapper mapper;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;

        public MessageProcessor(IMapper mapper, IHttpClientService httpClientService, IMappingService mappingService)
        {
            this.mapper = mapper;
            this.httpClientService = httpClientService;
            this.mappingService = mappingService;
        }

        public async Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageAction messageAction)
        {
            switch (messageContentType)
            {
                case MessageContentType.DigitalSkillsLevel:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchDigitalSkillsLevelServiceBusModel>(message);
                        var patchDigitalSkillModel = mapper.Map<PatchDigitalSkillModel>(serviceBusMessage);
                        patchDigitalSkillModel.MessageAction = messageAction;
                        patchDigitalSkillModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchDigitalSkillModel, "digitalSkillsLevel").ConfigureAwait(false);
                    }

                case MessageContentType.Restrictions:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchRestrictionsServiceBusModel>(message);
                        var patchRestrictionModel = mapper.Map<PatchRestrictionModel>(serviceBusMessage);
                        patchRestrictionModel.MessageAction = messageAction;
                        patchRestrictionModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchRestrictionModel, "restriction").ConfigureAwait(false);
                    }

                case MessageContentType.Skill:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchOnetSkillServiceBusModel>(message);
                        var patchRelatedSkillModel = mapper.Map<PatchOnetSkillModel>(serviceBusMessage);
                        patchRelatedSkillModel.MessageAction = messageAction;
                        patchRelatedSkillModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchRelatedSkillModel, "onetSkill").ConfigureAwait(false);
                    }

                case MessageContentType.SocSkillsMatrix:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchSkillsMatrixServiceBusModel>(message);
                        var patchSkillsMatrixModel = mapper.Map<PatchContextualisedModel>(serviceBusMessage);
                        patchSkillsMatrixModel.MessageAction = messageAction;
                        patchSkillsMatrixModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchSkillsMatrixModel, "skillsMatrix").ConfigureAwait(false);
                    }

                case MessageContentType.JobProfile:
                    return await ProcessJobProfileMessageAsync(message, messageAction, sequenceNumber).ConfigureAwait(false);

                default:
                    break;
            }

            return await Task.FromResult(HttpStatusCode.InternalServerError).ConfigureAwait(false);
        }

        private async Task<HttpStatusCode> ProcessJobProfileMessageAsync(string message, MessageAction messageAction, long sequenceNumber)
        {
            var jobProfile = mappingService.MapToSegmentModel(message, sequenceNumber);

            switch (messageAction)
            {
                case MessageAction.Draft:
                case MessageAction.Published:
                    var result = await httpClientService.PutAsync(jobProfile).ConfigureAwait(false);
                    if (result == HttpStatusCode.NotFound)
                    {
                        return await httpClientService.PostAsync(jobProfile).ConfigureAwait(false);
                    }

                    return result;

                case MessageAction.Deleted:
                    return await httpClientService.DeleteAsync(jobProfile.DocumentId).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageAction), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }
        }
    }
}