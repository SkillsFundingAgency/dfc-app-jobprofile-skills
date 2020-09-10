using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.API
{
    public class SkillsAPIResponse
    {
        public string digitalSkillsLevel { get; set; }

        public List<Skill> skills { get; set; }
        
        public RestrictionsAndRequirements restrictionsAndRequirements { get; set; }
    }
}
