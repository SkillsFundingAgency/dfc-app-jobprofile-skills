using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.MessageFunctionApp.Functions;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.UnitTests.Functions
{
    public class SitefinityMessageHandlerTests
    {
        private readonly SitefinityMessageHandler sitefinityMessageHandler;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly IMessageProcessor messageProcessor;
        private readonly IMessagePropertiesService messagePropertiesService;

        public SitefinityMessageHandlerTests()
        {
            logService = A.Fake<ILogService>();
            correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            messageProcessor = A.Fake<IMessageProcessor>();
            messagePropertiesService = A.Fake<IMessagePropertiesService>();

            sitefinityMessageHandler = new SitefinityMessageHandler(messageProcessor, messagePropertiesService, logService, correlationIdProvider);
        }

        public static IEnumerable<object[]> ValidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.OK },
            new object[] { HttpStatusCode.Created },
            new object[] { HttpStatusCode.AlreadyReported },
        };

        [Fact]
        public async Task RunHandlerThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sitefinityMessageHandler.Run(null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentExceptionWhenMessageBodyIsEmpty()
        {
            // Arrange
            var message = new Message(Encoding.ASCII.GetBytes(string.Empty));

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () => await sitefinityMessageHandler.Run(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageActionIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(messageAction: (MessageAction)999);

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sitefinityMessageHandler.Run(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageContentTypeIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(contentType: (MessageContentType)999);

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sitefinityMessageHandler.Run(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private Message CreateBaseMessage(MessageAction messageAction = MessageAction.Published, MessageContentType contentType = MessageContentType.JobProfile)
        {
            var message = A.Fake<Message>();
            message.Body = Encoding.ASCII.GetBytes("Some body json object here");
            message.UserProperties.Add("ActionType", messageAction.ToString());
            message.UserProperties.Add("CType", contentType);

            return message;
        }
    }
}