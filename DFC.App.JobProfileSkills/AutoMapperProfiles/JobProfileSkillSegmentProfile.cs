using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.ViewModels;

namespace DFC.App.JobProfileSkills.AutoMapperProfiles
{
    public class JobProfileSkillSegmentProfile : Profile
    {
        public JobProfileSkillSegmentProfile()
        {
            CreateMap<JobProfileSkillSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileSkillSegmentModel, DocumentViewModel>();

            CreateMap<JobProfileSkillSegmentModel, BodyViewModel>();

            CreateMap<JobProfileSkillSegmentDataModel, BodyDataViewModel>();

            CreateMap<JobProfileSkillSegmentSkillDataModel, BodyDataSkillSegmentSkillViewModel>();
        }
    }
}
