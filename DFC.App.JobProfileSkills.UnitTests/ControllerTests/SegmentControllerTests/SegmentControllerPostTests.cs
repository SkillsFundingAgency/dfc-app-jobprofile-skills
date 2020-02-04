using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPostTests : BaseSegmentController
    {
        [Fact]
        public async Task SegmentControllerPutReturnsBadResultWhenModelIsNull()
        {
            // Arrange
            var controller = BuildSegmentController();

            // Act
            var result = await controller.Post(null).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPutReturnsBadResultWhenModelIsInvalid()
        {
            // Arrange
            var jobProfileSkillSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController();

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Post(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPostReturnsAlreadyReportedIfDocumentAlreadyExists()
        {
            // Arrange
            var jobProfileSkillSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController();

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(jobProfileSkillSegmentModel);

            // Act
            var result = await controller.Post(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPostReturnsSuccessForCreate()
        {
            // Arrange
            var jobProfileSkillSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController();
            var expectedUpsertResponse = HttpStatusCode.Created;

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((JobProfileSkillSegmentModel)null);
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Post(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.Created, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}