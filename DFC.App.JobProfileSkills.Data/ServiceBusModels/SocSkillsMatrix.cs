using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels
{
    public class SocSkillsMatrix
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Contextualised { get; set; }

        public string ONetAttributeType { get; set; }

        public decimal? Rank { get; set; }

        public decimal? ONetRank { get; set; }

        public IEnumerable<RelatedSkill> RelatedSkill { get; set; }
    }
}