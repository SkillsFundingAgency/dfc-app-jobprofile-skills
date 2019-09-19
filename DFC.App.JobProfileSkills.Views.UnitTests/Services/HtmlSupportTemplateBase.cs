using RazorEngine.Templating;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Services
{
    public class HtmlSupportTemplateBase<T> : TemplateBase<T>
    {
        public HtmlSupportTemplateBase()
        {
            Html = new RazorHtmlHelper();
        }

        public RazorHtmlHelper Html { get; set; }
    }
}
