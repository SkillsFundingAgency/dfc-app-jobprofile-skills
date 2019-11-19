using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileSkills.Data.ServiceBusModels
{
    public class RefreshJobProfileSegmentServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string Segment { get; set; }
    }
}