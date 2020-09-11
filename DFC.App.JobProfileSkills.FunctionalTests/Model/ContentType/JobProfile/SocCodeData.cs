﻿using System.Collections.Generic;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.ContentType.JobProfile
{
    public class SocCodeData
    {
        public string Id { get; set; }

        public string SOCCode { get; set; }

        public string Description { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string UrlName { get; set; }

        public List<ApprenticeshipFramework> ApprenticeshipFramework { get; set; }

        public List<ApprenticeshipStandard> ApprenticeshipStandards { get; set; }
    }
}