using DFC.App.JobProfileSkills.ApiModels;
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
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        private const string ArticleName = "an-article-name";
        private readonly Guid documentId = Guid.NewGuid();

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var viewModel = GetBodyViewModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .Returns(viewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForJsonMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var apiModel = GetDummyApiModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<WhatItTakesApiModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .Returns(apiModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<WhatItTakesApiModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<WhatItTakesApiModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task ReturnsNotAcceptableForInvalidMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var viewModel = GetBodyViewModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .Returns(viewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsNoContentWhenDocumentDoesNotExist()
        {
            // Arrange
            var controller = BuildSegmentController();
            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored))
                .Returns((JobProfileSkillSegmentModel)null);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<WhatItTakesApiModel>(A<JobProfileSkillSegmentModel>.Ignored))
                .MustNotHaveHappened();

            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        private static WhatItTakesApiModel GetDummyApiModel()
        {
            return new WhatItTakesApiModel
            {
                DigitalSkillsLevel = "Digital skills level 1",
                RestrictionsAndRequirements = new RestrictionsAndRequirementsApiModel
                {
                    OtherRequirements = new List<string> { "Other requirement 1" },
                    RelatedRestrictions = new List<string> { "Related restriction 1" },
                },
                Skills = new List<RelatedSkillsApiModel>
                {
                    new RelatedSkillsApiModel
                    {
                        ONetRank = "Onet rank 1",
                        Description = "Onet Description 1",
                        ONetAttributeType = "ONetAttributeType 1",
                        ONetElementId = "ONetElementId 1",
                    },
                },
            };
        }

        private BodyViewModel GetBodyViewModel()
        {
            return new BodyViewModel
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = "job-profile-1",
                Data = new BodyDataViewModel
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