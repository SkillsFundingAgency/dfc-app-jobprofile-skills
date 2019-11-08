using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.ApiModels
{
    public class RestrictionsAndRequirementsApiModel
    {
        public List<string> RelatedRestrictions { get; set; }

        public List<string> OtherRequirements { get; set; }
    }
}