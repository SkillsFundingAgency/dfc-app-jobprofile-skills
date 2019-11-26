using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetAllTests
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly ISkillSegmentService skillSegmentService;

        public SegmentServiceGetAllTests()
        {
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            var mapper = A.Fake<AutoMapper.IMapper>();

            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task GetAllListReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<JobProfileSkillSegmentModel>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await skillSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResults, results);
        }

        [Fact]
        public async Task GetAllListReturnsNullWhenMissingRepository()
        {
            // arrange
            IEnumerable<JobProfileSkillSegmentModel> expectedResults = null;

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await skillSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResults, results);
        }
    }
}