using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;
using RelatedSkill = DFC.App.JobProfileSkills.Data.ServiceBusModels.RelatedSkill;
using Restriction = DFC.App.JobProfileSkills.Data.ServiceBusModels.Restriction;

namespace DFC.App.JobProfileSkills.MessageFunctionAppTests
{
    public class MappingServiceTests
    {
        private const int SequenceNumber = 123;
        private const string TestJobName = "Test Job name";
        private const string Title = "Title 1";
        private const string SocCodeId = "99";
        private const string DigitalSkillsLevel = "Some digital skills text";
        private const string OtherRequirements = "Other requirements text";
        private const string ContextualisedSkillTitle1 = "Related skill title 1";
        private const string ONetElementId1 = "ONetElementId 1";
        private const string ContextualisedDescription1 = "Contextualised 1";
        private const string ONetAttributeType1 = "ONetAttributeType 1";
        private const decimal ONetRank1 = 1.0M;
        private const decimal Rank1 = 5.0M;
        private const string SkillDescription1 = "Skill Description 1";
        private const string SkillTitle1 = "Skill Title 1";
        private const string RestrictionDescription1 = "Restriction Description 1";
        private const string RestrictionTitle1 = "Restriction Title 1";

        private static readonly DateTime LastModified = DateTime.UtcNow.AddDays(-1);
        private static readonly Guid JobProfileId = Guid.NewGuid();
        private static readonly Guid ContextualisedSkillId1 = Guid.NewGuid();
        private static readonly Guid OnetSkillId1 = Guid.NewGuid();
        private static readonly Guid RestrictionId1 = Guid.NewGuid();

        private readonly IMappingService mappingService;

        public MappingServiceTests()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new MessageFunctionApp.AutoMapperProfile.SkillsModelProfile());
            });

            var mapper = new Mapper(config);

            mappingService = new MappingService(mapper);
        }

        [Fact]
        public void MapToSegmentModelWhenJobProfileMessageSentThenItIsMappedCorrectly()
        {
            var fullJPMessage = BuildJobProfileMessage();
            var message = JsonConvert.SerializeObject(fullJPMessage);
            var expectedResponse = BuildExpectedResponse();

            // Act
            var actualMappedModel = mappingService.MapToSegmentModel(message, SequenceNumber);

            // Assert
            expectedResponse.Should().BeEquivalentTo(actualMappedModel);
        }

        private static JobProfileMessage BuildJobProfileMessage()
        {
            return new JobProfileMessage
            {
                JobProfileId = JobProfileId,
                CanonicalName = TestJobName,
                LastModified = LastModified,
                Title = Title,
                SocLevelTwo = SocCodeId,
                DigitalSkillsLevel = DigitalSkillsLevel,
                OtherRequirements = OtherRequirements,
                Restrictions = new List<Restriction>
                {
                    new Restriction
                    {
                        Id = RestrictionId1,
                        Title = RestrictionTitle1,
                        Info = RestrictionDescription1,
                    },
                },
                SocSkillsMatrixData = new List<SocSkillsMatrix>
                {
                    new SocSkillsMatrix
                    {
                        Id = ContextualisedSkillId1,
                        Title = ContextualisedSkillTitle1,
                        Contextualised = ContextualisedDescription1,
                        ONetAttributeType = ONetAttributeType1,
                        ONetRank = ONetRank1,
                        Rank = Rank1,
                        RelatedSkill = new List<RelatedSkill>
                        {
                            new RelatedSkill
                            {
                                Id = OnetSkillId1,
                                Description = SkillDescription1,
                                ONetElementId = ONetElementId1,
                                Title = SkillTitle1,
                            },
                        },
                    },
                },
            };
        }

        private static JobProfileSkillSegmentModel BuildExpectedResponse()
        {
            return new JobProfileSkillSegmentModel
            {
                CanonicalName = TestJobName,
                DocumentId = JobProfileId,
                SequenceNumber = SequenceNumber,
                SocLevelTwo = SocCodeId,
                Etag = null,
                Data = new JobProfileSkillSegmentDataModel
                {
                    DigitalSkill = DigitalSkillsLevel,
                    LastReviewed = LastModified,
                    OtherRequirements = OtherRequirements,
                    Restrictions = new List<Data.Models.Restriction>
                    {
                        new Data.Models.Restriction
                        {
                            Id = RestrictionId1,
                            Title = RestrictionTitle1,
                            Description = RestrictionDescription1,
                        },
                    },
                    Skills = new List<Skills>
                    {
                        new Skills
                        {
                            ContextualisedSkill = new ContextualisedSkill
                            {
                                Id = ContextualisedSkillId1,
                                ONetAttributeType = ONetAttributeType1,
                                Description = ContextualisedDescription1,
                                ONetRank = ONetRank1,
                                OriginalRank = Rank1,
                            },

                            OnetSkill = new OnetSkill
                            {
                                Id = OnetSkillId1,
                                Title = SkillTitle1,
                                ONetElementId = ONetElementId1,
                                Description = SkillDescription1,
                            },
                        },
                    },
                },
            };
        }
    }
}