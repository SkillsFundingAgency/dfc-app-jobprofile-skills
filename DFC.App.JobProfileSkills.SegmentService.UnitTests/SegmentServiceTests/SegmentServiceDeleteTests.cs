using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceDeleteTests
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly ISkillSegmentService skillSegmentService;

        private readonly Guid documentId = Guid.NewGuid();

        public SegmentServiceDeleteTests()
        {
            var mapper = A.Fake<AutoMapper.IMapper>();
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        public static IEnumerable<object[]> InvalidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.BadRequest },
            new object[] { HttpStatusCode.AlreadyReported },
            new object[] { HttpStatusCode.OK },
        };

        [Fact]
        public async Task DeleteReturnsSuccessWhenSegmentDeleted()
        {
            // arrange
            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            // act
            var result = await skillSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InvalidStatusCodes))]
        public async Task DeleteReturnsFalseWhenDocumentNotFound(HttpStatusCode statusCode)
        {
            // arrange
            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(statusCode);

            // act
            var result = await skillSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            Assert.False(result);
        }
    }
}