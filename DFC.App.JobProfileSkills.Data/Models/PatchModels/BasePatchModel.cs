using DFC.App.JobProfileSkills.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileSkills.Data.Models.PatchModels
{
    public class BasePatchModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public MessageAction MessageAction { get; set; }

        [Required]
        public long SequenceNumber { get; set; }
    }
}