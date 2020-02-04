using DFC.App.JobProfileSkills.Data.ServiceBusModels;

namespace DFC.App.JobProfileSkills.Data.Models.PatchModels
{
    public class PatchContextualisedModel : BasePatchModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ONetAttributeType { get; set; }

        public decimal OriginalRank { get; set; }

        public decimal ONetRank { get; set; }

        public RelatedSkill RelatedSkill { get; set; }
    }
}