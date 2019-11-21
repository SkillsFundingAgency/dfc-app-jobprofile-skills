using System;

namespace DFC.App.JobProfileSkills.ViewModels
{
    public class ContextualisedSkillViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string ONetAttributeType { get; set; }

        public decimal OriginalRank { get; set; }

        public decimal ONetRank { get; set; }
    }
}