using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.ApiModels
{
    public class WhatItTakesApiModel
    {
        public string DigitalSkillsLevel { get; set; }

        public List<RelatedSkillsApiModel> Skills { get; set; }

        public RestrictionsAndRequirementsApiModel RestrictionsAndRequirements { get; set; }
    }
}