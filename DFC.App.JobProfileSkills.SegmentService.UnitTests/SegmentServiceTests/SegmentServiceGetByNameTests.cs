using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
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
        private readonly IJobProfileSkillSegmentService jobProfileSkillSegmentService;

        public SegmentServiceGetByNameTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            jobProfileSkillSegmentService = new JobProfileSkillSegmentService(repository);
        }

        [Fact]
        public async Task GetByNameReturnsSuccess()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileSkillSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task GetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileSkillSegmentService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: canonicalName", exceptionResult.Message);
        }

        [Fact]
        public async Task GetByNameReturnsNullWhenMissingInRepository()
        {
            // arrange
            JobProfileSkillSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileSkillSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
