using AutoMapper;
using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.DraftSegmentService;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using DFC.App.JobProfileSkills.Repository.SitefinityApi;
using DFC.App.JobProfileSkills.SegmentService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace DFC.App.JobProfileSkills
{
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfileSegment";
        public const string SitefinityApiAppSettings = "SitefinityApi";
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey);
            var sitefinityApiConnection = configuration.GetSection(SitefinityApiAppSettings).Get<SitefinityAPIConnectionSettings>();

            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfileSkillSegmentModel>, CosmosRepository<JobProfileSkillSegmentModel>>();
            services.AddSingleton<IJobProfileSkillSegmentService, JobProfileSkillSegmentService>();
            services.AddSingleton<IDraftJobProfileSkillSegmentService, DraftJobProfileSkillSegmentService>();
            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Configure culture
            var supportedCultures = new[]
            {
                new CultureInfo("en-GB"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-GB"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();

            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}