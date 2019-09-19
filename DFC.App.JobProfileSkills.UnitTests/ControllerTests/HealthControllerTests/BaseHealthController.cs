using DFC.App.JobProfileSkills.Controllers;
using DFC.App.JobProfileSkills.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.HealthControllerTests
{
    public abstract class BaseHealthController
    {
        public BaseHealthController()
        {
            Logger = A.Fake<ILogger<HealthController>>();
            JobProfileSkillSegmentService = A.Fake<IJobProfileSkillSegmentService>();
        }

        protected IJobProfileSkillSegmentService JobProfileSkillSegmentService { get; }

        protected ILogger<HealthController> Logger { get; }

        protected HealthController BuildHealthController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(Logger, JobProfileSkillSegmentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
