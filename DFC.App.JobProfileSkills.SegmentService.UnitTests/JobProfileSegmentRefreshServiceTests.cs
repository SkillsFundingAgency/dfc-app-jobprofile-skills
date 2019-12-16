using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests
{
    public class JobProfileSegmentRefreshServiceTests
    {
        [Fact]
        public async Task SendMessageSendsMessageOnTopicClient()
        {
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var fakeCorrelationIdProvider = A.Fake<ICorrelationIdProvider>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, fakeCorrelationIdProvider);

            var model = new RefreshJobProfileSegmentServiceBusModel
            {
                CanonicalName = "some-canonical-name-1",
                JobProfileId = Guid.NewGuid(),
                Segment = "RelatedCareers",
            };

            // Act
            await refreshService.SendMessageAsync(model).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}