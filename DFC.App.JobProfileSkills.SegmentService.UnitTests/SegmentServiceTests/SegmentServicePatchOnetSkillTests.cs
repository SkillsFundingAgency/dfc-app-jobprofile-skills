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
    public class SegmentServicePatchOnetSkillTests
    {
        private readonly Guid defaultJobProfileId = Guid.NewGuid();
        private readonly Guid defaultOnetSkillId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchOnetSkillTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchOnetSkillReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchOnetSkillAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns((JobProfileSkillSegmentModel)null);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(GetPatchOnetSkillModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(GetPatchOnetSkillModel(), defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchOnetSkillModel(onetSkillId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileSkillSegmentModel();
            var patchModel = GetPatchOnetSkillModel(MessageAction.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(patchModel, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchOnetSkillModel();
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<OnetSkill>(A<PatchOnetSkillModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchOnetSkillReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchOnetSkillModel(MessageAction.Deleted);
            var existingModel = GetJobProfileSkillSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new SkillSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchOnetSkillAsync(model, defaultJobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<OnetSkill>(A<PatchOnetSkillModel>.Ignored)).MustNotHaveHappened();
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
                            OnetSkill = new OnetSkill
                            {
                                Id = defaultOnetSkillId,
                                Description = "Onet Description",
                                ONetElementId = "Onet Element Id",
                                Title = "Onet Title",
                            },
                        },
                    },
                },
            };
        }

        private PatchOnetSkillModel GetPatchOnetSkillModel(MessageAction messageAction = MessageAction.Published, Guid? onetSkillId = null)
        {
            return new PatchOnetSkillModel
            {
                JobProfileId = defaultJobProfileId,
                Id = onetSkillId ?? defaultOnetSkillId,
                Title = "Amended Onet skill",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Onet Description",
                ONetElementId = "Onet Element Id",
            };
        }
    }
}