using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Views.UnitTests.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfileOverview.Views.UnitTests.Tests
{
    public class BodyDataViewModelTests : TestBase
    {
        [Fact]
        public void ViewContainsRenderedContent()
        {
            //Arrange
            var model = new JobProfileSkillSegmentDataModel()
            {
                DigitalSkill = "DigitalSkill1",
            };

            var viewBag = new Dictionary<string, object>();
            var viewRenderer = new RazorEngineRenderer(ViewRootPath);

            //Act
            var viewRenderResponse = viewRenderer.Render(@"BodyData", model, viewBag);

            //Assert
            Assert.Contains(model.DigitalSkill, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
        }
    }
}
