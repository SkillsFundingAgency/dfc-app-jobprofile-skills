using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.MessageFunctionApp.Functions;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
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
        public static IEnumerable<object[]> ValidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.OK },
            new object[] { HttpStatusCode.Created },
            new object[] { HttpStatusCode.AlreadyReported },
        };

        [Fact]
        public async Task RunHandlerThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // Arrange
            var processor = A.Fake<IMessageProcessor>();
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            var logger = A.Fake<ILogger>();

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await SitefinityMessageHandler.Run(null, processor, messagePropertiesService, logger).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentExceptionWhenMessageBodyIsEmpty()
        {
            // Arrange
            var message = new Message(Encoding.ASCII.GetBytes(string.Empty));
            var processor = A.Fake<IMessageProcessor>();
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            var logger = A.Fake<ILogger>();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () => await SitefinityMessageHandler.Run(message, processor, messagePropertiesService, logger).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageActionIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(messageAction: (MessageAction)999);
            var processor = A.Fake<IMessageProcessor>();
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            var logger = A.Fake<ILogger>();

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await SitefinityMessageHandler.Run(message, processor, messagePropertiesService, logger).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageContentTypeIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(contentType: (MessageContentType)999);
            var processor = A.Fake<IMessageProcessor>();
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            var logger = A.Fake<ILogger>();

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await SitefinityMessageHandler.Run(message, processor, messagePropertiesService, logger).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerLogsWarningWhenMessageProcessorReturnsError()
        {
            // Arrange
            var message = CreateBaseMessage();
            var logger = A.Fake<ILogger>();

            var processor = A.Fake<IMessageProcessor>();
            A.CallTo(() => processor.ProcessAsync(A<string>.Ignored, A<long>.Ignored, A<MessageContentType>.Ignored, A<MessageAction>.Ignored)).Returns(HttpStatusCode.InternalServerError);

            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            A.CallTo(() => messagePropertiesService.GetSequenceNumber(A<Message>.Ignored)).Returns(123);

            // Act
            await SitefinityMessageHandler.Run(message, processor, messagePropertiesService, logger).ConfigureAwait(false);

            // Assert
            A.CallTo(() => logger.Log(LogLevel.Warning, 0, A<FormattedLogValues>.Ignored, A<Exception>.Ignored, A<Func<object, Exception, string>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ValidStatusCodes))]
        public async Task RunHandlerLogsInformationWhenMessageProcessorReturnsSuccess(HttpStatusCode status)
        {
            // Arrange
            var message = CreateBaseMessage();
            var logger = A.Fake<ILogger>();

            var processor = A.Fake<IMessageProcessor>();
            A.CallTo(() => processor.ProcessAsync(A<string>.Ignored, A<long>.Ignored, A<MessageContentType>.Ignored, A<MessageAction>.Ignored)).Returns(status);

            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            A.CallTo(() => messagePropertiesService.GetSequenceNumber(A<Message>.Ignored)).Returns(123);

            // Act
            await SitefinityMessageHandler.Run(message, processor, messagePropertiesService, logger).ConfigureAwait(false);

            // Assert
            A.CallTo(() => logger.Log(LogLevel.Information, 0, A<FormattedLogValues>.Ignored, A<Exception>.Ignored, A<Func<object, Exception, string>>.Ignored)).MustHaveHappenedTwiceExactly();
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