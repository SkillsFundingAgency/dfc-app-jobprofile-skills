using AutoMapper;
using DFC.App.JobProfileSkills.AutoMapperProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFC.App.JobProfileSkills.IntegrationTests.AutoMapperTests
{
    public class AutoMapperProfileTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public AutoMapperProfileTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public void AutoMapperProfileConfigurationForJobProfileModelProfileReturnSuccess()
        {
            // Arrange
            factory.CreateClient();
            var mapper = factory.Server.Host.Services.GetRequiredService<IMapper>();

            // Act
            mapper.ConfigurationProvider.AssertConfigurationIsValid<JobProfileSkillSegmentProfile>();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void AutoMapperProfileConfigurationForAllProfilesReturnSuccess()
        {
            // Arrange
            factory.CreateClient();
            var mapper = factory.Server.Host.Services.GetRequiredService<IMapper>();

            // Act
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            // Assert
            Assert.True(true);
        }
    }
}