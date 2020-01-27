using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPutTests : BaseSegmentController
    {
        [Fact]
        public async Task SegmentControllerPutReturnsBadResultWhenModelIsNull()
        {
            // Arrange
            var controller = BuildSegmentController();

            // Act
            var result = await controller.Put(null).ConfigureAwait(false);

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
            var result = await controller.Put(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPutReturnsNotFoundWhenDocumentDoesNotAlreadyExist()
        {
            // Arrange
            var jobProfileSkillSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController();

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((JobProfileSkillSegmentModel)null);

            // Act
            var result = await controller.Put(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPutReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = new JobProfileSkillSegmentModel { SequenceNumber = 999 };
            var modelToUpsert = new JobProfileSkillSegmentModel { SequenceNumber = 124 };
            var controller = BuildSegmentController();

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPutReturnsSuccessForUpdate()
        {
            // Arrange
            var existingModel = new JobProfileSkillSegmentModel { SequenceNumber = 123 };
            var modelToUpsert = new JobProfileSkillSegmentModel { SequenceNumber = 124 };

            var controller = BuildSegmentController();

            var expectedUpsertResponse = HttpStatusCode.OK;

            A.CallTo(() => FakeSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}