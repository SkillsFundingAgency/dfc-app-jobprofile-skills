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

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchSkillsMatrixTests
    {
        private readonly Guid defaultJobProfileId = Guid.NewGuid();
        private readonly Guid defaultSkillsMatrixId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchSkillsMatrixTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchSkillsMatrixAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns((JobProfileSkillSegmentModel)null);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(GetPatchContextualisedModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(GetPatchContextualisedModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchContextualisedModel(skillsMatrixId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchContextualisedModel(MessageAction.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchContextualisedModel();
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Skills>(A<PatchContextualisedModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchSkillsMatrixReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchContextualisedModel(MessageAction.Deleted);
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchSkillsMatrixAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Skills>(A<PatchContextualisedModel>.Ignored)).MustNotHaveHappened();
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
                    Skills = new List<Skills>
                    {
                        new Skills
                        {
                            ContextualisedSkill = new ContextualisedSkill
                            {
                                Id = defaultSkillsMatrixId,
                                Description = "Contextualised Description",
                                ONetAttributeType = "ONetAttributeType 1",
                                ONetRank = 1,
                                OriginalRank = 2,
                            },
                        },
                    },
                },
            };
        }

        private PatchContextualisedModel GetPatchContextualisedModel(MessageAction messageAction = MessageAction.Published, Guid? skillsMatrixId = null)
        {
            return new PatchContextualisedModel
            {
                JobProfileId = defaultJobProfileId,
                Id = skillsMatrixId ?? defaultSkillsMatrixId,
                Title = "Amended Contextualised skill",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Contextualised Description",
                ONetAttributeType = "ONetAttributeType",
                ONetRank = 1,
                OriginalRank = 2,
                RelatedSkill = new RelatedSkill
                {
                    Id = Guid.NewGuid(),
                    Description = "Related skill 1",
                    Title = "Related skill title 1",
                    ONetElementId = "Related skill Onet element 1",
                },
            };
        }
    }
}