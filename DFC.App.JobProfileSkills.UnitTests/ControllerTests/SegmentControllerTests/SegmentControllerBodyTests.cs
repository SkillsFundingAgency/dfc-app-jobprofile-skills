using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
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

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

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

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task ReturnsNotAcceptableForInvalidMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}