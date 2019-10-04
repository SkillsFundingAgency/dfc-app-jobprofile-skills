using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetByIdTests
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly IJobProfileSkillSegmentService jobProfileSkillSegmentService;

        public SegmentServiceGetByIdTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            jobProfileSkillSegmentService = new JobProfileSkillSegmentService(repository);
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsSuccess()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileSkillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            var documentId = Guid.NewGuid();
            JobProfileSkillSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileSkillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}