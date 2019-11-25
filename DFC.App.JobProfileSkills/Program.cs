﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfileSkills
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}