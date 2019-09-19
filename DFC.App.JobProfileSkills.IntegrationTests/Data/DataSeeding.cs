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

            Created = DateTime.UtcNow;
        }

        public Guid Article1Id { get; private set; }

        public Guid Article2Id { get; private set; }

        public Guid Article3Id { get; private set; }

        public string Article1Name { get; private set; }

        public string Article2Name { get; private set; }

        public string Article3Name { get; private set; }

        public DateTime Created { get; private set; }

        public async Task AddData(CustomWebApplicationFactory<Startup> factory)
        {
            var url = $"/{Segment}";
            var models = CreateModels();

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                await client.PostAsync(url, model, new JsonMediaTypeFormatter()).ConfigureAwait(false);
            }
        }

        public async Task RemoveData(CustomWebApplicationFactory<Startup> factory)
        {
            var models = CreateModels();

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                var url = string.Concat("/", Segment, "/", model.DocumentId);
                await client.DeleteAsync(url).ConfigureAwait(false);
            }
        }

        private List<JobProfileSkillSegmentModel> CreateModels()
        {
            var models = new List<JobProfileSkillSegmentModel>()
            {
                new JobProfileSkillSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article1Id,
                    CanonicalName = Article1Name,
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill1",
                        LastReviewed = Created.Subtract(TimeSpan.FromDays(1)),
                        Restrictions = new List<string>() { "Restrictions1a", "Restrictions1b" },
                        OtherRequirements = "OtherRequirements1",
                        RestrictionsSummary = "RestrictionsSummary1",
                        Skills = new List<JobProfileSkillSegmentSkillDataModel>()
                        {
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id1",
                                ContextualisedDescription = "ContextualisedDescription1",
                                Rank = 1,
                                StandardDescription = "StandardDescription1",
                            },
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id2",
                                ContextualisedDescription = "ContextualisedDescription2",
                                Rank = 2,
                                StandardDescription = "StandardDescription2",
                            },
                        },
                        Summary = "Summary1",
                    },
                },
                new JobProfileSkillSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article2Id,
                    CanonicalName = Article2Name,
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill2",
                        LastReviewed = Created.Subtract(TimeSpan.FromDays(1)),
                        Restrictions = new List<string>() { "Restrictions2a", "Restrictions2b" },
                        OtherRequirements = "OtherRequirements2",
                        RestrictionsSummary = "RestrictionsSummary2",
                        Skills = new List<JobProfileSkillSegmentSkillDataModel>()
                        {
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id3",
                                ContextualisedDescription = "ContextualisedDescription3",
                                Rank = 3,
                                StandardDescription = "StandardDescription3",
                            },
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id4",
                                ContextualisedDescription = "ContextualisedDescription4",
                                Rank = 4,
                                StandardDescription = "StandardDescription4",
                            },
                        },
                        Summary = "Summary2",
                    },
                },
                new JobProfileSkillSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article3Id,
                    CanonicalName = Article3Name,
                    Data = new JobProfileSkillSegmentDataModel
                    {
                        DigitalSkill = "DigitalSkill3",
                        LastReviewed = Created.Subtract(TimeSpan.FromDays(1)),
                        Restrictions = new List<string>() { "Restrictions3a", "Restrictions3b" },
                        OtherRequirements = "OtherRequirements3",
                        RestrictionsSummary = "RestrictionsSummary3",
                        Skills = new List<JobProfileSkillSegmentSkillDataModel>()
                        {
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id5",
                                ContextualisedDescription = "ContextualisedDescription5",
                                Rank = 5,
                                StandardDescription = "StandardDescription5",
                            },
                            new JobProfileSkillSegmentSkillDataModel()
                            {
                                Id = "id6",
                                ContextualisedDescription = "ContextualisedDescription6",
                                Rank = 6,
                                StandardDescription = "StandardDescription6",
                            },
                        },
                    },
                },
            };

            return models;
        }
    }
}
