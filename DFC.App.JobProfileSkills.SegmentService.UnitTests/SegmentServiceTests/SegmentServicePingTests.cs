using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using FakeItEasy;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePingTests
    {
        [Fact]
        public void PingReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            var expectedResult = true;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var skillSegmentService = new JobProfileSkillSegmentService(repository);

            // act
            var result = skillSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void PingReturnsFalseWhenNotInRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<JobProfileSkillSegmentModel>>();
            var expectedResult = false;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var skillSegmentService = new JobProfileSkillSegmentService(repository);

            // act
            var result = skillSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}