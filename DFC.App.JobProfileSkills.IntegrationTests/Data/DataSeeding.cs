using DFC.App.JobProfileSkills.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.IntegrationTests.Data
{
    public class DataSeeding
    {
        private const string Segment = "segment";

        public DataSeeding()
        {
            Article1Id = Guid.NewGuid();
            Article2Id = Guid.NewGuid();
            Article3Id = Guid.NewGuid();

            Article1Name = Article1Id.ToString();
            Article2Name = Article2Id.ToString();
            Article3Name = Article3Id.ToString();
            Article2SocCode = "23456";
        }

        public Guid Article1Id { get; private set; }

        public Guid Article2Id { get; private set; }

        public Guid Article3Id { get; private set; }

        public string Article1Name { get; private set; }

        public string Article2Name { get; private set; }

        public string Article3Name { get; private set; }

        public string Article2SocCode { get; private set; }

        public async Task AddData(CustomWebApplicationFactory<Startup> factory)
        {
            var url = $"/{Segment}";
            var models = CreateModels();

            var client = factory?.CreateClient();

            client?.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                await client.PostAsync(url, model, new JsonMediaTypeFormatter()).ConfigureAwait(false);
            }
        }

        public async Task RemoveData(CustomWebApplicationFactory<Startup> factory)
        {
            var models = CreateModels();

            var client = factory?.CreateClient();

            client?.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                var url = string.Concat("/", Segment, "/", model.DocumentId);
                await client.DeleteAsync(url).ConfigureAwait(false);
            }
        }

        private List<JobProfileSkillSegmentModel> CreateModels()
        {
            return new List<JobProfileSkillSegmentModel>
            {
                new JobProfileSkillSegmentModel
                {
                    DocumentId = Article1Id,
                    CanonicalName = Article1Name,
                    SocLevelTwo = "12345",
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill1",
                        Restrictions = new List<Restriction>
                        {
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions1a", Description = "Restrictions1a", Rank = 1 },
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions1b", Description = "Restrictions1b", Rank = 2 },
                        },
                        OtherRequirements = "OtherRequirements1",
                        Skills = new List<Skills>
                        {
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc1",
                                    Title = "OnetSkill1Title",
                                    ONetElementId = "OnetElementId1",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc1",
                                    ONetAttributeType = "ContextualisedSkill attribute type 1",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc2",
                                    Title = "OnetSkill2Title",
                                    ONetElementId = "OnetElementId2",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc2",
                                    ONetAttributeType = "ContextualisedSkill attribute type 2",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                        },
                    },
                },
                new JobProfileSkillSegmentModel
                {
                    DocumentId = Article2Id,
                    CanonicalName = Article2Name,
                    SocLevelTwo = Article2SocCode,
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill2",
                        Restrictions = new List<Restriction>
                        {
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions2a", Description = "Restrictions2a", Rank = 1 },
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions2b", Description = "Restrictions2b", Rank = 2 },
                        },
                        OtherRequirements = "OtherRequirements2",
                        Skills = new List<Skills>
                        {
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc3",
                                    Title = "OnetSkill3Title",
                                    ONetElementId = "OnetElementId3",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc3",
                                    ONetAttributeType = "ContextualisedSkill attribute type 3",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc4",
                                    Title = "OnetSkill4Title",
                                    ONetElementId = "OnetElementId4",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc4",
                                    ONetAttributeType = "ContextualisedSkill attribute type 4",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                        },
                    },
                },
                new JobProfileSkillSegmentModel
                {
                    DocumentId = Article3Id,
                    CanonicalName = Article3Name,
                    SocLevelTwo = "34567",
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill3",
                        Restrictions = new List<Restriction>
                        {
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions3a", Description = "Restrictions3a", Rank = 1 },
                            new Restriction { Id = Guid.NewGuid(), Title = "Restrictions3b", Description = "Restrictions3b", Rank = 2 },
                        },
                        OtherRequirements = "OtherRequirements3",
                        Skills = new List<Skills>
                        {
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc5",
                                    Title = "OnetSkill5Title",
                                    ONetElementId = "OnetElementId5",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc5",
                                    ONetAttributeType = "ContextualisedSkill attribute type 5",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                            new Skills
                            {
                                OnetSkill = new OnetSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "OnetSkillDesc6",
                                    Title = "OnetSkill6Title",
                                    ONetElementId = "OnetElementId6",
                                },
                                ContextualisedSkill = new ContextualisedSkill
                                {
                                    Id = Guid.NewGuid(),
                                    Description = "ContextualisedSkillDesc6",
                                    ONetAttributeType = "ContextualisedSkill attribute type 6",
                                    OriginalRank = 123.0M,
                                    ONetRank = 4.0M,
                                },
                            },
                        },
                    },
                },
            };
        }
    }
}