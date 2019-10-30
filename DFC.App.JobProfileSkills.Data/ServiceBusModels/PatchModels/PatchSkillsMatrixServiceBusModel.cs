using System;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels
{
    public class PatchSkillsMatrixServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Contextualised { get; set; }

        public string ONetAttributeType { get; set; }

        public decimal Rank { get; set; }

        public decimal ONetRank { get; set; }

        public RelatedSkill RelatedSkill { get; set; }
    }
}