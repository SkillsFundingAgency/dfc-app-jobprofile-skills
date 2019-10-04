using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerUpsertTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpsertReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var jobProfileSkillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = BuildExpectedUpsertResponse(A.Fake<JobProfileSkillSegmentModel>());

            A.CallTo(() => FakeJobProfileSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Save(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpsertReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var jobProfileSkillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = BuildExpectedUpsertResponse(A.Fake<JobProfileSkillSegmentModel>(), HttpStatusCode.OK);

            A.CallTo(() => FakeJobProfileSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Save(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSkillSegmentService.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpsertReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Save(null).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpsertReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var relatedCareersSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Save(relatedCareersSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        private UpsertJobProfileSkillsModelResponse BuildExpectedUpsertResponse(JobProfileSkillSegmentModel model, HttpStatusCode status = HttpStatusCode.Created)
        {
            return new UpsertJobProfileSkillsModelResponse
            {
                JobProfileSkillSegmentModel = model,
                ResponseStatusCode = status,
            };
        }
    }
}