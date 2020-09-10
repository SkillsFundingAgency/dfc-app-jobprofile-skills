using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.ContentType.JobProfile;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.Support;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus;
using DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support
{
    public class SetUpAndTearDown
    {
        protected CommonAction CommonAction { get; set; } = new CommonAction();

        protected AppSettings AppSettings { get; set; }

        protected JobProfileContentType WakeUpJobProfile { get; set; }

        protected JobProfileContentType JobProfile { get; set; }

        protected ServiceBusSupport ServiceBus { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Get settings from appsettings
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            this.AppSettings = configuration.Get<AppSettings>();

            this.ServiceBus = new ServiceBusSupport(new TopicClientFactory(), this.AppSettings);

            // Send wake up job profile
            this.WakeUpJobProfile = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            this.WakeUpJobProfile.JobProfileId = Guid.NewGuid().ToString();
            this.WakeUpJobProfile.CanonicalName = this.CommonAction.RandomString(10).ToLowerInvariant();
            var jobProfileMessageBody = this.CommonAction.ConvertObjectToByteArray(this.WakeUpJobProfile);
            var message = new MessageFactory().Create(this.WakeUpJobProfile.JobProfileId, jobProfileMessageBody, "Published", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMinutes(this.AppSettings.DeploymentWaitInMinutes)).ConfigureAwait(true);

            // Generate a test job profile
            var restriction = new Restriction()
            {
                Info = "This is automated information for the restriction content type",
                Id = Guid.NewGuid().ToString(),
                Title = "This is an automated restriction title",
            };

            var skill = new RelatedSkill()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "This is an automated skill description",
                ONetElementId = "1.A.2.b",
                Title = "This is an automated skill title"
            };

            var socSkillsMatrix = new SocSkillsMatrixDataContentType()
            {
                Id = Guid.NewGuid().ToString(),
                Contextualised = string.Empty,
                ONetAttributeType = "Knowledge",
                ONetRank = 4.94,
                Rank = 1.0,
                RelatedSkill = new System.Collections.Generic.List<RelatedSkill>() { skill },
                RelatedSOC = new System.Collections.Generic.List<RelatedSOC>() { },
                Title = "This is an automated socSkillsMatrix"
            };

            this.JobProfile = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            this.JobProfile.JobProfileId = Guid.NewGuid().ToString();
            this.JobProfile.CanonicalName = this.CommonAction.RandomString(10).ToLowerInvariant();
            this.JobProfile.Restrictions.Add(restriction);
            this.JobProfile.SocSkillsMatrixData.Add(socSkillsMatrix);

            // Send job profile to the service bus
            jobProfileMessageBody = this.CommonAction.ConvertObjectToByteArray(this.JobProfile);
            message = new MessageFactory().Create(this.JobProfile.JobProfileId, jobProfileMessageBody, "Published", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Delete wake up job profile
            var wakeUpJobProfileDelete = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            wakeUpJobProfileDelete.JobProfileId = this.WakeUpJobProfile.JobProfileId;
            var messageBody = this.CommonAction.ConvertObjectToByteArray(wakeUpJobProfileDelete);
            var message = new MessageFactory().Create(this.WakeUpJobProfile.JobProfileId, messageBody, "Deleted", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);

            // Delete test job profile
            var jobProfileDelete = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            jobProfileDelete.JobProfileId = this.JobProfile.JobProfileId;
            messageBody = this.CommonAction.ConvertObjectToByteArray(jobProfileDelete);
            message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Deleted", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
        }
    }
}
