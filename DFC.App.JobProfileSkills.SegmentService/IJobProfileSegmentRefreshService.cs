using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.SegmentService
{
    public interface IJobProfileSegmentRefreshService<in TModel>
    {
        Task SendMessageAsync(TModel model);
    }
}