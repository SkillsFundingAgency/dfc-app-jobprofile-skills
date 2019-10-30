﻿using System;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels
{
    public class PatchRelatedSkillServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string ONetElementId { get; set; }

        public string Description { get; set; }
    }
}