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
    public class SegmentControllerRoutePostTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IClassFixture<DataSeeding>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly DataSeeding dataSeeding;

        public SegmentControllerRoutePostTests(CustomWebApplicationFactory<Startup> factory, DataSeeding dataSeeding)
        {
            this.factory = factory;
            this.dataSeeding = dataSeeding;

            if (dataSeeding == null)
            {
                throw new ArgumentNullException(nameof(dataSeeding));
            }

            dataSeeding.AddData(factory).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task WhenAddingNewArticleReturnCreated()
        {
            // Arrange
            var url = "/segment";
            var documentId = Guid.NewGuid();
            var skillSegmentModel = new JobProfileSkillSegmentModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12PostSoc",
                Data = new JobProfileSkillSegmentDataModel(),
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, skillSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Cleanup
            await client.DeleteAsync(string.Concat(url, "/", documentId)).ConfigureAwait(false);
        }

        [Fact]
        public async Task WhenUpdateExistingArticleReturnsAlreadyReported()
        {
            // Arrange
            var url = "/segment";
            var skillSegmentModel = new JobProfileSkillSegmentModel()
            {
                DocumentId = dataSeeding.Article2Id,
                CanonicalName = "article2_modified",
                SocLevelTwo = dataSeeding.Article2SocCode,
                Data = new JobProfileSkillSegmentDataModel(),
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, skillSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.AlreadyReported);
        }
    }
}