using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileSkills.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerCreateOrUpdateTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var skillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            JobProfileSkillSegmentModel existingJobProfileSkillSegmentModel = null;
            var createdJobProfileSkillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileSkillSegmentModel);
            A.CallTo(() => JobProfileSkillSegmentService.CreateAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(createdJobProfileSkillSegmentModel);

            // Act
            var result = await controller.CreateOrUpdate(skillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => JobProfileSkillSegmentService.CreateAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            A.Equals((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var jobProfileSkillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            var existingJobProfileSkillSegmentModel = A.Fake<JobProfileSkillSegmentModel>();
            JobProfileSkillSegmentModel updatedSkillSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileSkillSegmentModel);
            A.CallTo(() => JobProfileSkillSegmentService.ReplaceAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(updatedSkillSegmentModel);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSkillSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => JobProfileSkillSegmentService.ReplaceAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkObjectResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobProfileSkillSegmentModel jobProfileSkillSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var jobProfileSkillSegmentModel = new JobProfileSkillSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.CreateOrUpdate(jobProfileSkillSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
