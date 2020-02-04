using DFC.App.JobProfileSkills.Controllers;
using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.SegmentService;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public abstract class BaseSegmentController
    {
        public BaseSegmentController()
        {
            FakeLogger = A.Fake<ILogService>();
            FakeSkillSegmentService = A.Fake<ISkillSegmentService>();
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            FakeJobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogService FakeLogger { get; }

        protected ISkillSegmentService FakeSkillSegmentService { get; }

        protected IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> FakeJobProfileSegmentRefreshService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected SegmentController BuildSegmentController(string mediaTypeName = MediaTypeNames.Application.Json)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new SegmentController(FakeLogger, FakeSkillSegmentService, FakeMapper, FakeJobProfileSegmentRefreshService)
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