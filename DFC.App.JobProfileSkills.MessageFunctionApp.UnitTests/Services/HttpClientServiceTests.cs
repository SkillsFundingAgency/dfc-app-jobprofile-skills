using DFC.App.JobProfileSkills.Data.Enums;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.MessageFunctionApp.Models;
using DFC.App.JobProfileSkills.MessageFunctionApp.Services;
using DFC.App.JobProfileSkills.MessageFunctionApp.UnitTests.ClientHandlers;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.UnitTests.Services
{
    public class HttpClientServiceTests
    {
        private readonly SegmentClientOptions options;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private Mock<IHttpClientFactory> mockFactory;

        public HttpClientServiceTests()
        {
            options = new SegmentClientOptions
            {
                BaseAddress = new Uri("http://baseAddress"),
                Timeout = TimeSpan.MinValue,
            };

            logService = A.Fake<ILogService>();
            correlationIdProvider = A.Fake<ICorrelationIdProvider>();
        }

        public Mock<IHttpClientFactory> CreateClientFactory(HttpClient httpClient)
        {
            mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient()).Returns(httpClient);
            return mockFactory;
        }

        [Fact]
        public async Task PostAsyncReturnsOKStatusCodeWhenHttpResponseIsSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            var result = await httpClientService.PostAsync(GetSegmentModel()).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PostAsyncReturnsExceptionWhenHttpResponseIsNotSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent("Unsuccessful content") };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PostAsync(GetSegmentModel()).ConfigureAwait(false)).ConfigureAwait(false);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PutAsyncReturnsExceptionWhenHttpResponseIsNotSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent("Unsuccessful content") };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PutAsync(GetSegmentModel()).ConfigureAwait(false)).ConfigureAwait(false);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PutAsyncReturnsStatusWhenHttpResponseIsNotFound()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent("Unsuccessful content") };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            var result = await httpClientService.PutAsync(GetSegmentModel()).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PutAsyncReturnsOKStatusCodeWhenHttpResponseIsSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            var result = await httpClientService.PutAsync(GetSegmentModel()).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PatchAsyncReturnsOKStatusCodeWhenHttpResponseIsSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            var result = await httpClientService.PatchAsync(GetPatchModel(), "DummyPatchEndpoint").ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PatchAsyncReturnsExceptionWhenHttpResponseIsNotSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("Unsuccessful content") };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PatchAsync(GetPatchModel(), "DummyPatchEndpoint").ConfigureAwait(false)).ConfigureAwait(false);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task DeleteAsyncReturnsOKStatusCodeWhenHttpResponseIsSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            var result = await httpClientService.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task DeleteAsyncReturnsExceptionWhenHttpResponseIsNotSuccessful()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("Unsuccessful content") };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://SomeDummyUrl") };
            var httpClientService = new HttpClientService(options, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
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

        private PatchDigitalSkillModel GetPatchModel()
        {
            return new PatchDigitalSkillModel
            {
                JobProfileId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Title = "title 1",
                MessageAction = MessageAction.Published,
                SequenceNumber = 123,
            };
        }
    }
}