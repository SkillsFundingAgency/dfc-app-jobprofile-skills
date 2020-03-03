using DFC.App.JobProfileSkills.Views.UnitTests.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Tests
{
    public class TestBase
    {
        public string ViewRootPath => "..\\..\\..\\..\\DFC.App.JobProfileSkills\\";

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