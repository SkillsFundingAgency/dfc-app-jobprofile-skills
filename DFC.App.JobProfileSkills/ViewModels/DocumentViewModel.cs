﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileSkills.ViewModels
{
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        public BodyDataViewModel Data { get; set; }
    }
}