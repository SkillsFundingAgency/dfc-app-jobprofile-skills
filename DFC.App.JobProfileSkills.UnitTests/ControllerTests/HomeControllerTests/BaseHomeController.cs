using DFC.App.JobProfileSkills.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.HomeControllerTests
{
    public abstract class BaseHomeController
    {
        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        protected HomeController BuildHomeController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HomeController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}