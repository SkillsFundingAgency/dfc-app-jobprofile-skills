using DFC.App.JobProfileSkills.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Tests
{
    public class BodyDataSkillsViewTests : TestBase
    {
        private const string ViewName = "BodyDataSkills";

        [Fact]
        public void ContainsSkillsContent()
        {
            //Arrange
            var model = new BodyDataViewModel
            {
                DigitalSkill = "DigitalSkill1",
                Skills = new List<SkillsViewModel>
                {
                    new SkillsViewModel
                    {
                        ContextualisedSkill = new ContextualisedSkillViewModel
                        {
                            Id = Guid.NewGuid(),
                            Description = "contextualised Description",
                            ONetAttributeType = "ONetAttributeType1",
                            ONetRank = 1,
                            OriginalRank = 2,
                        },
                    },
                },
            };

            //Act
            var viewRenderResponse = RenderView(model, ViewName);

            //Assert
            Assert.Contains(model.DigitalSkill, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            foreach (var skill in model.Skills)
            {
                Assert.Contains(skill.ContextualisedSkill.Description, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ContainsStandardDescriptionWhenContextualisedDescriptionIsNullOrWhitespace(string contextualisedDescription)
        {
            //Arrange
            var model = new BodyDataViewModel
            {
                DigitalSkill = "DigitalSkill1",
                Skills = new List<SkillsViewModel>
                {
                    new SkillsViewModel
                    {
                        OnetSkill = new OnetSkillViewModel
                        {
                            Id = Guid.NewGuid(),
                            Description = "StandardDescription1",
                            Title = "StandardTitle1",
                            ONetElementId = "StandardOnetElement1",
                        },
                        ContextualisedSkill = new ContextualisedSkillViewModel
                        {
                            Id = Guid.NewGuid(),
                            Description = contextualisedDescription,
                            ONetAttributeType = "ONetAttributeType1",
                            ONetRank = 1,
                            OriginalRank = 2,
                        },
                    },
                },
            };

            //Act
            var viewRenderResponse = RenderView(model, ViewName);

            //Assert
            Assert.Contains(model.Skills.First().OnetSkill.Description, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ContainsNoContentIfNoSkills()
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