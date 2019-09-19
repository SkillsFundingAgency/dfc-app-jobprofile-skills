using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerDeleteTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsSuccessWhenDocumentIdExists(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();
            expectedResult.DocumentId = documentId;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(documentId)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSkillSegmentService.DeleteAsync(documentId, A<int>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsNotFoundWhenDocumentIdDoesNotExist(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            JobProfileSkillSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(documentId)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(documentId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => JobProfileSkillSegmentService.DeleteAsync(documentId, A<int>.Ignored)).MustNotHaveHappened();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
