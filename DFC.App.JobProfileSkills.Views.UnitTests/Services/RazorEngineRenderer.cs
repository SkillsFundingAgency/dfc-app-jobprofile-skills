﻿using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Collections.Generic;
using System.IO;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Services
{
    public class RazorEngineRenderer : IViewRenderer
    {
        private readonly string viewRootPath;

        public RazorEngineRenderer(string viewRootPath)
        {
            this.viewRootPath = viewRootPath;
        }

        public string Render(string templateValue, object model, IDictionary<string, object> viewBag)
        {
            var result = string.Empty;

            var razorConfig = new TemplateServiceConfiguration()
            {
                TemplateManager = CreateTemplateManager(),
                BaseTemplateType = typeof(HtmlSupportTemplateBase<>),
            };
            razorConfig.Namespaces.Add("DFC.App.JobProfileSkills.ViewModels");

            using (var razorEngine = RazorEngineService.Create(razorConfig))
            {
                var dynamicViewBag = new DynamicViewBag(viewBag);
                result = razorEngine.RunCompile(templateValue, model.GetType(), model, dynamicViewBag);
            }

            return result;
        }

        private ITemplateManager CreateTemplateManager()
        {
            var directories = Directory.GetDirectories(viewRootPath, "*.*", SearchOption.AllDirectories);
            return new ResolvePathTemplateManager(directories);
        }
    }
}