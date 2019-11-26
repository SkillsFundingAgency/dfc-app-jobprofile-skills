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
            SkillSegmentService = A.Fake<ISkillSegmentService>();
        }

        protected ISkillSegmentService SkillSegmentService { get; }

        protected ILogger<HealthController> Logger { get; }

        protected HealthController BuildHealthController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(Logger, SkillSegmentService)
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
