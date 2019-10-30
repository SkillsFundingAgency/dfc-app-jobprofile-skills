using DFC.App.JobProfileSkills.Data.Models;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Services
{
    public interface IMappingService
    {
        JobProfileSkillSegmentModel MapToSegmentModel(string message, long sequenceNumber);
    }
}