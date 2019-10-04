using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileSkills.ViewModels
{
    public class BodyDataViewModel
    {
        public BodyDataViewModel()
        {
            Skills = new List<BodyDataSkillSegmentSkillViewModel>();
        }

        public DateTime LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<BodyDataSkillSegmentSkillViewModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<GenericListContent> Restrictions { get; set; }

        public string OtherRequirements { get; set; }

        public bool HasRestrictions => Restrictions != null && Restrictions.Any();
    }
}