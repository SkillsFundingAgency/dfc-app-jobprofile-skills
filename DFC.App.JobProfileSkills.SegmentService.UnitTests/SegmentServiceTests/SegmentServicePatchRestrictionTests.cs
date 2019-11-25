using AutoMapper;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Restriction = DFC.App.JobProfileSkills.Data.Models.Restriction;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchRestrictionTests
    {
        private readonly Guid defaultJobProfileId = Guid.NewGuid();
        private readonly Guid defaultRestrictionId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchRestrictionTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchRestrictionReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchRestrictionAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchRestrictionReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns((JobProfileSkillSegmentModel)null);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(GetPatchRestrictionModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchRestrictionReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(GetPatchRestrictionModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchRestrictionReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchRestrictionModel(skillsMatrixId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchRestrictionReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchRestrictionModel(MessageAction.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchRestrictionReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchRestrictionModel();
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Restriction>(A<PatchRestrictionModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchRestrictionReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchRestrictionModel(MessageAction.Deleted);
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchRestrictionAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Restriction>(A<PatchRestrictionModel>.Ignored)).MustNotHaveHappened();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        private JobProfileSkillSegmentModel GetJobProfileSkillSegmentModel(int sequenceNumber = 1)
        {
            return new JobProfileSkillSegmentModel
            {
                DocumentId = defaultJobProfileId,
                SequenceNumber = sequenceNumber,
                Data = new JobProfileSkillSegmentDataModel
                {
                    Restrictions = new List<Restriction>
                    {
                        new Restriction
                        {
                            Id = defaultRestrictionId,
                            Description = "Restriction desc 1",
                            Title = "Restriction title 1",
                        },
                    },
                },
            };
        }

        private PatchRestrictionModel GetPatchRestrictionModel(MessageAction messageAction = MessageAction.Published, Guid? skillsMatrixId = null)
        {
            return new PatchRestrictionModel
            {
                JobProfileId = defaultJobProfileId,
                Id = skillsMatrixId ?? defaultRestrictionId,
                Title = "Amended restriction",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Restriction desc 1",
            };
        }
    }
}