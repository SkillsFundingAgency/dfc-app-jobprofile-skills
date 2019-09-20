using DFC.App.JobProfileSkills.Views.UnitTests.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Tests
{
    public class TestBase
    {
        private readonly string viewRootPath;
        private readonly IConfigurationRoot configuration;

        public TestBase()
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            configuration = config.Build();

            viewRootPath = configuration["ViewRootPath"];
        }

        public string ViewRootPath => viewRootPath;

        protected string CurrencySymbol => CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

        protected string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value);
        }

        protected string RenderView(object model, string viewName)
        {
            var viewBag = new Dictionary<string, object>();
            var viewRenderer = new RazorEngineRenderer(ViewRootPath);
            return viewRenderer.Render(viewName, model, viewBag);
        }
    }
}
