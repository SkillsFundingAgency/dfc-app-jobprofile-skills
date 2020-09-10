using DFC.App.CareerPath.IntegrationTests.API.Model.Support;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Model.Support
{
    public class AppSettings
    {
        public int DeploymentWaitInMinutes { get; set; }

        public ServiceBusConfig ServiceBusConfig { get; set; } = new ServiceBusConfig();

        public APIConfig APIConfig { get; set; } = new APIConfig();
    }
}
