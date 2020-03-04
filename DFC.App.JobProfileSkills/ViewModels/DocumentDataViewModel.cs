using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DFC.App.JobProfileSkills.ViewModels
{
    public class DocumentDataViewModel
    {
        [Display(Name = "Last Updated")]
        public DateTime LastReviewed { get; set; }

        public string OtherRequirements { get; set; }

        public string DigitalSkill { get; set; }

        public IEnumerable<SkillsViewModel> Skills { get; set; }

        public IEnumerable<RestrictionViewModel> Restrictions { get; set; }

        public bool HasRestrictions => Restrictions != null && Restrictions.Any();

        public bool HasSkills => Skills != null && Skills.Any();
    }
}