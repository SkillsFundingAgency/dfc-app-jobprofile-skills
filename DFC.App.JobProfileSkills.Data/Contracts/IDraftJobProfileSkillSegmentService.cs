using DFC.App.JobProfileSkills.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Data.Contracts
{
    public interface IDraftJobProfileSkillSegmentService
    {
        Task<JobProfileSkillSegmentModel> GetSitefinityData(string canonicalName);
    }
}
