using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Extensions;
using DFC.App.JobProfileSkills.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogService logService;
        private readonly ISkillSegmentService skillSegmentService;

        public HealthController(ILogService logService, ISkillSegmentService skillSegmentService)
        {
            this.logService = logService;
            this.skillSegmentService = skillSegmentService;
        }

        [HttpGet]
        [Route("{controller}")]
        public async Task<IActionResult> Health()
        {
            var resourceName = typeof(Program).Namespace;
            string message;

            logService.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await skillSegmentService.PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    message = "Document store is available";
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    return this.NegotiateContentResult(viewModel);
                }

                message = $"Ping to {resourceName} has failed";
                logService.LogError($"{nameof(Health)}: {message}");
            }
            catch (Exception ex)
            {
                message = $"{resourceName} exception: {ex.Message}";
                logService.LogError($"{nameof(Health)}: {message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("{controller}/ping")]
        public IActionResult Ping()
        {
            logService.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private static HealthViewModel CreateHealthViewModel(string resourceName, string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}