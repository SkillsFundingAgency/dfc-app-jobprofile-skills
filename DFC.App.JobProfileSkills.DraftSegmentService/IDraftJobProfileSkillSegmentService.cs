using DFC.App.JobProfileSkills.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.DraftSegmentService
{
    public interface IDraftJobProfileSkillSegmentService
    {
        Task<JobProfileSkillSegmentModel> GetSitefinityData(string canonicalName);
    }
}