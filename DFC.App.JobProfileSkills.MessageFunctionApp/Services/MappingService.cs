using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using Newtonsoft.Json;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper mapper;

        public MappingService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public JobProfileSkillSegmentModel MapToSegmentModel(string message, long sequenceNumber)
        {
            var fullJobProfileMessage = JsonConvert.DeserializeObject<JobProfileMessage>(message);
            var fullJobProfile = mapper.Map<JobProfileSkillSegmentModel>(fullJobProfileMessage);
            fullJobProfile.SequenceNumber = sequenceNumber;

            return fullJobProfile;
        }
    }
}