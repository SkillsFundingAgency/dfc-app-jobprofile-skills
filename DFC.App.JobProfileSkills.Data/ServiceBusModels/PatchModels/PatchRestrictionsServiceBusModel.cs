using System;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels
{
    public class PatchRestrictionsServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Info { get; set; }
    }
}