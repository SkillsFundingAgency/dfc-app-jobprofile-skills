using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels
{
    public class JobProfileMessage : BaseJobProfileMessage
    {
        public string Title { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public DateTime LastModified { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public string OtherRequirements { get; set; }

        public string DigitalSkillsLevel { get; set; }

        public IEnumerable<Restriction> Restrictions { get; set; }

        public IEnumerable<SocSkillsMatrix> SocSkillsMatrixData { get; set; }
    }
}