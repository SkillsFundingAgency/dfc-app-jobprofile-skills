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
    public class SegmentControllerPatchOnetSkillTests : BaseSegmentController
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
        public async Task SegmentControllerPatchOnetSkillReturnsBadRequestWhenModelIsNull()
        {
            // Arrange
            var controller = BuildSegmentController();

            // Act
            var result = await controller.PatchOnetSkill(null, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPatchOnetSkillReturnsBadRequestWhenModelIsInvalid()
        {
            // Arrange
            var model = new PatchOnetSkillModel();
            var controller = BuildSegmentController();
            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.PatchOnetSkill(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidStatusCodes))]
        public async Task SegmentControllerPatchOnetSkillReturnsResponseAndLogsErrorWhenInvalidStatusReturned(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchOnetSkillModel();

            A.CallTo(() => FakeSkillSegmentService.PatchOnetSkillAsync(A<PatchOnetSkillModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchOnetSkill(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.PatchOnetSkillAsync(A<PatchOnetSkillModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(ValidStatusCodes))]
        public async Task SegmentControllerPatchOnetSkillReturnsSuccessForSuccessfulResponse(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchOnetSkillModel();

            A.CallTo(() => FakeSkillSegmentService.PatchOnetSkillAsync(A<PatchOnetSkillModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchOnetSkill(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.PatchOnetSkillAsync(A<PatchOnetSkillModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}