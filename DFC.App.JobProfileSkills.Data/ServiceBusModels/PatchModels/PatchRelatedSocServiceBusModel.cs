using System;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels
{
    public class PatchRelatedSocServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public Guid SocSkillMatrixId { get; set; }

        public string SOCCode { get; set; }
    }
}