using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.DraftSegmentService
{
    public class DraftJobProfileSkillSegmentService : IDraftJobProfileSkillSegmentService
    {
        public Task<JobProfileSkillSegmentModel> GetSitefinityData(string canonicalName)
        {
            throw new NotImplementedException();
        }
    }
}
