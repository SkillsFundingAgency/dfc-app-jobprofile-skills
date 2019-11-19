using RazorEngine.Text;

namespace DFC.App.JobProfileSkills.Views.UnitTests.Services
{
    public class RazorHtmlHelper
    {
        public IEncodedString Raw(string rawString)
        {
            return new RawString(rawString);
        }
    }
}
