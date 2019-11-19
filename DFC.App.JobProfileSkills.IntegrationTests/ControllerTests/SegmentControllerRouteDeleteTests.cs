using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.IntegrationTests.Data;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.IntegrationTests.ControllerTests
{
    public class SegmentControllerRouteDeleteTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IClassFixture<DataSeeding>
    {
        private const string SegmentUrl = "/segment";

        private readonly CustomWebApplicationFactory<Startup> factory;

        public SegmentControllerRouteDeleteTests(CustomWebApplicationFactory<Startup> factory, DataSeeding dataSeeding)
        {
            this.factory = factory;

            if (dataSeeding == null)
            {
                throw new ArgumentNullException(nameof(dataSeeding));
            }

            dataSeeding?.AddData(factory).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task NonExistingSegmentReturnsNotFound()
        {
            // Arrange
            var uri = new Uri($"/segment/{Guid.NewGuid()}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingSegmentReturnsOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var deleteUri = new Uri($"{SegmentUrl}/{documentId}", UriKind.Relative);

            var skillSegmentModel = new JobProfileSkillSegmentModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12PostSoc",
                Data = new JobProfileSkillSegmentDataModel(),
            };

            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            await client.PostAsync(SegmentUrl, skillSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.DeleteAsync(deleteUri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}