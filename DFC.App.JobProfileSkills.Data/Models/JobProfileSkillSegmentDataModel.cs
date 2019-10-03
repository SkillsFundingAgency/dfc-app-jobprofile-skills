using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileSkills.Data.Models
{
    public class JobProfileSkillSegmentDataModel
    {
        public DateTime LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<JobProfileSkillSegmentSkillDataModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<GenericListContent> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
    }
}