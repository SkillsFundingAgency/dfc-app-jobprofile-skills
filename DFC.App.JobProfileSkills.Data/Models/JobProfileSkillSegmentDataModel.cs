using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Data.Models
{
    public class JobProfileSkillSegmentDataModel
    {
        public JobProfileSkillSegmentDataModel()
        {
            Skills = new List<JobProfileSkillSegmentSkillDataModel>();
            Restrictions = new List<string>();
        }

        public DateTime LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<JobProfileSkillSegmentSkillDataModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<string> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
    }
}
