using System.Net;

namespace DFC.App.JobProfileSkills.Data.Models
{
    public class UpsertJobProfileSkillsModelResponse
    {
        public JobProfileSkillSegmentModel JobProfileSkillSegmentModel { get; set; }

        public HttpStatusCode ResponseStatusCode { get; set; }
    }
}