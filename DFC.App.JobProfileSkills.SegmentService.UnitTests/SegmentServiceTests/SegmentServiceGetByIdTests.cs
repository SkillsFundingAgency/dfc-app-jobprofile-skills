﻿using DFC.App.JobProfileSkills.Data.Contracts;
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
    public class SegmentServiceGetByIdTests
    {
        private readonly ICosmosRepository<JobProfileSkillSegmentModel> repository;
        private readonly ISkillSegmentService skillSegmentService;

        public SegmentServiceGetByIdTests()
        {
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            var mapper = A.Fake<AutoMapper.IMapper>();

            repository = A.Fake<ICosmosRepository<JobProfileSkillSegmentModel>>();
            skillSegmentService = new SkillSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task GetByIdReturnsSuccess()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileSkillSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await skillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            var documentId = Guid.NewGuid();
            JobProfileSkillSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await skillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileSkillSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }
    }
}