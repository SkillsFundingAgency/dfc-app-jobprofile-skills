using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerDocumentTests : BaseSegmentController
    {
        private const string Article = "an-article-name";

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var bodyViewModel = GetBodyViewModel();

            A.CallTo(() => FakeSkillSegmentService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).Returns(documentViewModel);

            // Act
            var result = await controller.Document(Article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            JobProfileSkillSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeSkillSegmentService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Document(Article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        private BodyViewModel GetBodyViewModel()
        {
            return new BodyViewModel
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = Article,
                SequenceNumber = 123,
                Data = new DocumentDataViewModel
                {
                    DigitalSkill = "Digital skill 1",
                    LastReviewed = DateTime.UtcNow,
                    OtherRequirements = "Other requirements",
                    Restrictions = new List<RestrictionViewModel>
                    {
                        new RestrictionViewModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            Description = "Restriction Desc 1",
                            Title = "Restriction title 1",
                        },
                    },
                    Skills = new List<SkillsViewModel>
                    {
                        new SkillsViewModel
                        {
                            OnetSkill = new OnetSkillViewModel
                            {
                                Id = Guid.NewGuid(),
                                Description = "Onet desc",
                                Title = "Onet title",
                                ONetElementId = "Onet element Id",
                            },
                            ContextualisedSkill = new ContextualisedSkillViewModel
                            {
                                Id = Guid.NewGuid(),
                                Description = "Contextualised desc 1",
                                ONetAttributeType = "Onet attribute 1",
                                ONetRank = 1,
                                OriginalRank = 2,
                            },
                        },
                    },
                },
            };
        }
    }
}