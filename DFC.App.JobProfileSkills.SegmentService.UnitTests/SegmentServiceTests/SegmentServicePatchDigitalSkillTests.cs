using AutoMapper;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchDigitalSkillTests
    {
        private readonly Guid jobProfileId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchDigitalSkillTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchDigitalSkillReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchDigitalSkillAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchDigitalSkillReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns((JobProfileSkillSegmentModel)null);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchDigitalSkillAsync(GetPatchDigitalSkillModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchDigitalSkillReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchDigitalSkillAsync(GetPatchDigitalSkillModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchDigitalSkillReturnsSuccess()
        {
            // Arrange
            var model = GetPatchDigitalSkillModel();
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            var result = await segmentService.PatchDigitalSkillAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        private JobProfileSkillSegmentModel GetJobProfileSkillSegmentModel(int sequenceNumber = 1)
        {
            return new JobProfileSkillSegmentModel
            {
                DocumentId = jobProfileId,
                SequenceNumber = sequenceNumber,
                Data = new JobProfileSkillSegmentDataModel
                {
                    DigitalSkill = "Existing Digital Skill",
                },
            };
        }

        private PatchDigitalSkillModel GetPatchDigitalSkillModel()
        {
            return new PatchDigitalSkillModel
            {
                JobProfileId = jobProfileId,
                Id = Guid.NewGuid(),
                Title = "Amended digital skill",
                MessageAction = MessageAction.Published,
                SequenceNumber = 123,
            };
        }
    }
}