using DFC.App.JobProfileSkills.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileSkills.Data.Models
{
    public class JobProfileSkillSegmentModel : IDataModel
    {
        public JobProfileSkillSegmentModel()
        {
            Data = new JobProfileSkillSegmentDataModel();
        }

        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public int PartitionKey => Created.Second;

        public JobProfileSkillSegmentDataModel Data { get; set; }
    }
}
