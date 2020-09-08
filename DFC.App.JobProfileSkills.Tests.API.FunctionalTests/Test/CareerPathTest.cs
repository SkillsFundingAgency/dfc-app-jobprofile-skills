using DFC.App.CareerPath.FunctionalTests.Support;
using DFC.App.CareerPath.FunctionalTests.Support.API;
using DFC.App.CareerPath.FunctionalTests.Support.API.RestFactory;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.App.CareerPath.FunctionalTests.Test
{
    public class CareerPathTest : SetUpAndTearDown
    {
        private ICareerPathAPI careerPathApi;

        [SetUp]
        public void SetUp()
        {
            this.careerPathApi = new CareerPathAPI(new RestClientFactory(), new RestRequestFactory(), this.AppSettings);
        }

        [Test]
        public async Task CareerPathJobProfile()
        {
            var response = await this.careerPathApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);
            Assert.AreEqual(this.JobProfile.CareerPathAndProgression, response.Data.CareerPathAndProgression[0]);
        }
    }
}