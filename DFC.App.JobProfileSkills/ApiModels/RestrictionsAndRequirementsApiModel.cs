using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.ApiModels
{
    public class RestrictionsAndRequirementsApiModel
    {
        public List<string> RelatedRestrictions { get; set; }

        public List<string> OtherRequirements { get; set; }
    }
}