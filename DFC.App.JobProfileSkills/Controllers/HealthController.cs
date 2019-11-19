﻿using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Extensions;
using DFC.App.JobProfileSkills.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;
        private readonly IJobProfileSkillSegmentService jobProfileSkillSegmentService;

        public HealthController(ILogger<HealthController> logger, IJobProfileSkillSegmentService jobProfileSkillSegmentService)
        {
            this.logger = logger;
            this.jobProfileSkillSegmentService = jobProfileSkillSegmentService;
        }

        [HttpGet]
        [Route("{controller}")]
        public async Task<IActionResult> Health()
        {
            var resourceName = typeof(Program).Namespace;
            string message;

            logger.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await jobProfileSkillSegmentService.PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    message = "Document store is available";
                    logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    return this.NegotiateContentResult(viewModel);
                }

                message = $"Ping to {resourceName} has failed";
                logger.LogError($"{nameof(Health)}: {message}");
            }
            catch (Exception ex)
            {
                message = $"{resourceName} exception: {ex.Message}";
                logger.LogError(ex, $"{nameof(Health)}: {message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("{controller}/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

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