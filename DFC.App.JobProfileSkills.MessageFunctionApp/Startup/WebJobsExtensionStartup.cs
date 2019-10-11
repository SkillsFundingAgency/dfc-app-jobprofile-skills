using DFC.App.JobProfileSkills.MessageFunctionApp.Models;
using DFC.App.JobProfileSkills.MessageFunctionApp.Startup;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Startup
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var segmentClientOptions = configuration.GetSection("JobProfileSkillsSegmentClientOptions").Get<SegmentClientOptions>();

            builder.AddDependencyInjection();

            builder.Services.AddSingleton<SegmentClientOptions>(segmentClientOptions);
            builder.Services.AddSingleton<HttpClient>(new HttpClient());
        }
    }
}