﻿using FakeItEasy;
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
            const bool documentExists = true;
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSkillSegmentService.DeleteAsync(documentId)).Returns(documentExists);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsNotFoundWhenDocumentIdDoesNotExist(string mediaTypeName)
        {
            // Arrange
            const bool documentExists = false;
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSkillSegmentService.DeleteAsync(documentId)).Returns(documentExists);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            var statusResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}