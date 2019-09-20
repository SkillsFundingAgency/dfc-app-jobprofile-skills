using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.ViewModels
{
    public class BodyDataViewModel
    {
        public BodyDataViewModel()
        {
            Skills = new List<BodyDataSkillSegmentSkillViewModel>();
            Restrictions = new List<string>();
        }

        public DateTime LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<BodyDataSkillSegmentSkillViewModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<string> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
   }
}
