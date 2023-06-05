using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.MessageFunctionApp.Models;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly SegmentClientOptions segmentClientOptions;
        private readonly HttpClient httpClient;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public HttpClientService(SegmentClientOptions segmentClientOptions, IHttpClientFactory httpClientFactory, ILogService logService, ICorrelationIdProvider correlationIdProvider)
        {
            this.segmentClientOptions = segmentClientOptions;
            this.httpClient = httpClientFactory.CreateClient();
            this.logService = logService;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task<HttpStatusCode> PostAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            logService.LogInformation($"{nameof(PostAsync)} has been called with JobProfileSkillSegmentModel {nameof(skillSegmentModel)}");

            var url = new Uri($"{segmentClientOptions?.BaseAddress}segment");
            ConfigureHttpClient();

            using (var content = new ObjectContent(typeof(JobProfileSkillSegmentModel), skillSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST, Id: {skillSegmentModel?.DocumentId}.");
                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PutAsync(JobProfileSkillSegmentModel skillSegmentModel)
        {
            logService.LogInformation($"{nameof(PutAsync)} has been called with JobProfileSkillSegmentModel {nameof(skillSegmentModel)}");

            var url = new Uri($"{segmentClientOptions?.BaseAddress}segment");
            ConfigureHttpClient();

            using (var content = new ObjectContent(typeof(JobProfileSkillSegmentModel), skillSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PutAsync(url, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for Put type {typeof(JobProfileSkillSegmentModel)}, Id: {skillSegmentModel?.DocumentId}");
                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel
        {
            logService.LogInformation($"{nameof(PatchAsync)} has been called with patchModel {nameof(patchModel)}");

            var url = new Uri($"{segmentClientOptions.BaseAddress}segment/{patchModel?.JobProfileId}/{patchTypeEndpoint}");
            ConfigureHttpClient();

            using (var content = new ObjectContent<T>(patchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PatchAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for patch type {typeof(T)}, Id: {patchModel?.JobProfileId}");

                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            logService.LogInformation($"{nameof(DeleteAsync)} has been called with id {nameof(id)}");

            var url = new Uri($"{segmentClientOptions?.BaseAddress}segment/{id}");
            ConfigureHttpClient();
            var response = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for DELETE, Id: {id}.");
                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        private void ConfigureHttpClient()
        {
            logService.LogInformation($"{nameof(ConfigureHttpClient)} has been called");

            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                logService.LogInformation($"{nameof(ConfigureHttpClient)} does not contain {nameof(HeaderName.CorrelationId)}");

                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
            }
        }
    }
}