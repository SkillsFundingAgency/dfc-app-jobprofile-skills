using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Data.Models
{
    public class JobProfileSkillSegmentDataModel
    {
        public const string SegmentName = "WhatItTakes";

        public DateTime LastReviewed { get; set; }

        public string OtherRequirements { get; set; }

        public string DigitalSkill { get; set; }

        public IList<Skills> Skills { get; set; }

        public IList<Restriction> Restrictions { get; set; }
    }
}