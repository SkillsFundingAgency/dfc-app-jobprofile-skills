using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetByNameTests
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly ISkillSegmentService skillSegmentService;

        public SegmentServiceGetByNameTests()
        {
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            var mapper = A.Fake<AutoMapper.IMapper>();

            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task GetByNameReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await skillSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await skillSegmentService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'canonicalName')", exceptionResult.Message);
        }

        [Fact]
        public async Task GetByNameReturnsNullWhenMissingInRepository()
        {
            // arrange
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns((JobProfileSkillSegmentModel)null);

            // act
            var result = await skillSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);
        }
    }
}