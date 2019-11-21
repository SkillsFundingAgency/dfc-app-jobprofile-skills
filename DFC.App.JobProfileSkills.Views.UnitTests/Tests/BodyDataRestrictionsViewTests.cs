using DFC.App.JobProfileSkills.ViewModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Tests
{
    public class BodyDataRestrictionsViewTests : TestBase
    {
        private const string ViewName = "BodyDataRestrictions";

        [Fact]
        public void ContainsRestrictionsContent()
        {
            //Arrange
            var model = new BodyDataViewModel()
            {
                OtherRequirements = "OtherRequirements1",
                Restrictions = new List<RestrictionViewModel>
                {
                    new RestrictionViewModel { Id = "1", Description = "Description1" },
                    new RestrictionViewModel { Id = "2", Description = "Description2" },
                },
            };

            //Act
            var viewRenderResponse = RenderView(model, ViewName);

            //Assert
            Assert.Contains(model.OtherRequirements, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            foreach (var restriction in model.Restrictions)
            {
                Assert.Contains(restriction.Description, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void ContainsNoContentIfNoRestrictions()
        {
            //Arrange
            var model = new BodyDataViewModel();

            //Act
            var viewRenderResponse = RenderView(model, ViewName);

            //Assert
            Assert.Empty(viewRenderResponse);
        }
    }
}