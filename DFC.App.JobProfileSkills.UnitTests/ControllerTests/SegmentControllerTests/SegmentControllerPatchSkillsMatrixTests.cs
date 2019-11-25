using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPatchSkillsMatrixTests : BaseSegmentController
    {
        public static IEnumerable<object[]> ValidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.OK },
            new object[] { HttpStatusCode.Created },
        };

        public static IEnumerable<object[]> InvalidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.BadRequest },
            new object[] { HttpStatusCode.AlreadyReported },
            new object[] { HttpStatusCode.NotFound },
        };

        [Fact]
        public async Task SegmentControllerPatchSkillsMatrixReturnsBadRequestWhenModelIsNull()
        {
            // Arrange
            var controller = BuildSegmentController();

            // Act
            var result = await controller.PatchSkillsMatrix(null, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPatchSkillsMatrixReturnsBadRequestWhenModelIsInvalid()
        {
            // Arrange
            var model = new PatchContextualisedModel();
            var controller = BuildSegmentController();
            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.PatchSkillsMatrix(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidStatusCodes))]
        public async Task SegmentControllerPatchSkillsMatrixReturnsResponseAndLogsErrorWhenInvalidStatusReturned(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchContextualisedModel();

            A.CallTo(() => FakeSkillSegmentService.PatchSkillsMatrixAsync(A<PatchContextualisedModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchSkillsMatrix(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.PatchSkillsMatrixAsync(A<PatchContextualisedModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(ValidStatusCodes))]
        public async Task SegmentControllerPatchSkillsMatrixReturnsSuccessForSuccessfulResponse(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchContextualisedModel();

            A.CallTo(() => FakeSkillSegmentService.PatchSkillsMatrixAsync(A<PatchContextualisedModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchSkillsMatrix(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.PatchSkillsMatrixAsync(A<PatchContextualisedModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}