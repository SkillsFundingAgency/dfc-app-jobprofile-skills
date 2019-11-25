using AutoMapper;
using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.UnitTests.Services
{
    public class MessageProcessorTests
    {
        private const long SequenceNumber = 123;
        private const string BaseMessage = "Dummy Serialised Message";
        private const int InvalidEnumValue = 999;

        private readonly IMessageProcessor processor;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;
        private readonly IMapper mapper;

        public MessageProcessorTests()
        {
            var expectedMappedModel = GetSegmentModel();
            mapper = A.Fake<IMapper>();

            mappingService = A.Fake<IMappingService>();
            A.CallTo(() => mappingService.MapToSegmentModel(A<string>.Ignored, A<long>.Ignored)).Returns(expectedMappedModel);

            httpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            processor = new MessageProcessor(mapper, httpClientService, mappingService);
        }

        [Fact]
        public async Task ProcessAsyncReturnsInternalServerErrorWhenInvalidMessageContentTypeSent()
        {
            // Act
            var result = await processor
                .ProcessAsync(BaseMessage, SequenceNumber, (MessageContentType)InvalidEnumValue, MessageAction.Published)
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public async Task ProcessAsyncArgumentOutOfRangeExceptionWhenInvalidMessageActionSent()
        {
            await Assert.ThrowsAnyAsync<ArgumentOutOfRangeException>(async () => await processor.ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, (MessageAction)InvalidEnumValue).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ProcessAsyncCallsDeleteAsyncWhenDeletedMessageActionSent()
        {
            // Act
            var result = await processor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageAction.Deleted)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPutAsyncAndReturnsOkResultWhenDataExists()
        {
            // Arrange
            var putHttpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => putHttpClientService.PutAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var putMessageProcessor = new MessageProcessor(mapper, putHttpClientService, mappingService);

            // Act
            var result = await putMessageProcessor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageAction.Published)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => putHttpClientService.PutAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPostAsyncAndReturnsOkResultWhenDataDoesntExist()
        {
            // Arrange
            var postHttpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => postHttpClientService.PutAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => postHttpClientService.PostAsync(A<JobProfileSkillSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var postMessageProcessor = new MessageProcessor(mapper, postHttpClientService, mappingService);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageAction.Published)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => postHttpClientService.PutAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => postHttpClientService.PostAsync(A<JobProfileSkillSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchDigitalSkillsLevelContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchDigitalSkillModel>(A<PatchDigitalSkillsLevelServiceBusModel>.Ignored)).Returns(A.Fake<PatchDigitalSkillModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchDigitalSkillModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.DigitalSkillsLevel, MessageAction.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchDigitalSkillModel>(A<PatchDigitalSkillsLevelServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchDigitalSkillModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchRestrictionModelContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchRestrictionModel>(A<PatchRestrictionsServiceBusModel>.Ignored)).Returns(A.Fake<PatchRestrictionModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchRestrictionModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.Restriction, MessageAction.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchRestrictionModel>(A<PatchRestrictionsServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchRestrictionModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchOnetSkillModelContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchOnetSkillModel>(A<PatchOnetSkillServiceBusModel>.Ignored)).Returns(A.Fake<PatchOnetSkillModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchOnetSkillModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.Skill, MessageAction.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchOnetSkillModel>(A<PatchOnetSkillServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchOnetSkillModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchSocSkillsMatrixContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchContextualisedModel>(A<PatchSkillsMatrixServiceBusModel>.Ignored)).Returns(A.Fake<PatchContextualisedModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchContextualisedModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.SocSkillsMatrix, MessageAction.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchContextualisedModel>(A<PatchSkillsMatrixServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchContextualisedModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        private JobProfileSkillSegmentModel GetSegmentModel()
        {
            return new JobProfileSkillSegmentModel
            {
                DocumentId = Guid.NewGuid(),
                SequenceNumber = 1,
                SocLevelTwo = "12",
                CanonicalName = "job-1",
                Data = new JobProfileSkillSegmentDataModel
                {
                    DigitalSkill = "Digital skill 1",
                    LastReviewed = DateTime.UtcNow,
                    OtherRequirements = "Other requirements 1",
                    Restrictions = new List<Restriction>
                    {
                        new Restriction
                        {
                            Id = Guid.NewGuid(),
                            Title = "Restriction title 1",
                            Description = "Restriction description 1",
                        },
                    },
                    Skills = new List<Skills>
                    {
                        new Skills
                        {
                            ContextualisedSkill = new ContextualisedSkill
                            {
                                Id = Guid.NewGuid(),
                                Description = "contextualised description 1",
                                ONetRank = 3.0M,
                                ONetAttributeType = "ONetAttributeType1",
                                OriginalRank = 5.0M,
                            },
                            OnetSkill = new OnetSkill
                            {
                                Id = Guid.NewGuid(),
                                Description = "Onet description 1",
                                Title = "Onet Title 1",
                                ONetElementId = "ONetElementId 1",
                            },
                        },
                    },
                },
            };
        }
    }
}