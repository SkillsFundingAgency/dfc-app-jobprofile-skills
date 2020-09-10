using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.ContentType.JobProfile;
using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.MessageBody
{
    public class SocSkillsMatrixMessageBody
    {
        public string Contextualised { get; set; }
        public string ONetAttributeType { get; set; }
        public double Rank { get; set; }
        public double ONetRank { get; set; }
        public RelatedSkill RelatedSkill { get; set; }
        public List<RelatedSOC> RelatedSOC { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string JobProfileId { get; set; }
        public string JobProfileTitle { get; set; }
    }


}
