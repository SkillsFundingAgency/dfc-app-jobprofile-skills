using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.MessageBody;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.API.RestFactory;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Test
{
    public class SkillsTest : SetUpAndTearDown
    {
        private ISkillsAPI skillsApi;

        [SetUp]
        public void SetUp()
        {
            this.skillsApi = new SkillsAPI(new RestClientFactory(), new RestRequestFactory(), this.AppSettings);
        }

        [Test]
        public async Task RestrictionCType()
        {
            var restrictionMessageBody = new RestrictionMessageBody()
            {
                Id = this.JobProfile.Restrictions[0].Id,
                Info = "This is updated information",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Title = "This is an updated title",
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(restrictionMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "Restriction");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var response = await this.skillsApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual(restrictionMessageBody.Info, response.Data.restrictionsAndRequirements.relatedRestrictions[0]);
        }

        [Test]
        public async Task SkillCType()
        {
            var skillMessageBody = new SkillMessageBody()
            {
                Id = this.JobProfile.SocSkillsMatrixData[0].RelatedSkill[0].Id,
                Description = "This is an updated skill description",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                ONetElementId = this.JobProfile.SocSkillsMatrixData[0].RelatedSkill[0].ONetElementId,
                SocSkillMatrixId = this.JobProfile.SocSkillsMatrixData[0].Id,
                SocSkillMatrixTitle = this.JobProfile.SocSkillsMatrixData[0].Title,
                Title = this.JobProfile.SocSkillsMatrixData[0].RelatedSkill[0].Title,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(skillMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "Skill");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var response = await this.skillsApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual(skillMessageBody.Description, response.Data.skills[0].description);
        }

        [Test]
        public async Task SocSkillsMatrixCType()
        {
            var skillMessageBody = new SocSkillsMatrixMessageBody()
            {
                Id = this.JobProfile.SocSkillsMatrixData[0].Id,
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Contextualised = string.Empty,
                ONetAttributeType = "Knowledge",
                ONetRank = 4.94,
                Rank = 1.0,
                RelatedSkill = new Model.ContentType.JobProfile.RelatedSkill()
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "This is a new skill",
                    ONetElementId = "1.B.2.c",
                    Title = "This is a new skill title",
                },
                RelatedSOC = new System.Collections.Generic.List<Model.ContentType.JobProfile.RelatedSOC>()
                {
                    new Model.ContentType.JobProfile.RelatedSOC()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SOCCode = this.JobProfile.SocCodeData.SOCCode,
                    },
                },
                Title = this.JobProfile.SocSkillsMatrixData[0].RelatedSkill[0].Title,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(skillMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "SocSkillsMatrix");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var response = await this.skillsApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual(skillMessageBody.RelatedSkill.Description, response.Data.skills[0].description);
        }
    }
}