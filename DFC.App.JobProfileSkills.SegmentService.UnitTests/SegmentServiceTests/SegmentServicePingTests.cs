﻿using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Repository.CosmosDb;
using FakeItEasy;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileSkills.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePingTests
    {
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;
        private readonly IMapper mapper;

        public SegmentServicePingTests()
        {
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async Task PingReturnsSuccess()
        {
            // arrange
            const bool expectedResult = true;
            var repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // act
            var result = await skillSegmentService.PingAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task PingReturnsFalseWhenNotInRepository()
        {
            // arrange
            const bool expectedResult = false;
            var repository = A.Dummy<ICosmosRepository<JobProfileSkillSegmentModel>>();

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // act
            var result = await skillSegmentService.PingAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }
    }
}