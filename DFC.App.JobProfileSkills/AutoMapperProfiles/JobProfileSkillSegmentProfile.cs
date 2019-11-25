using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfileSkills.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobProfileSkillSegmentProfile : Profile
    {
        public JobProfileSkillSegmentProfile()
        {
            CreateMap<JobProfileSkillSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileSkillSegmentModel, DocumentViewModel>();

            CreateMap<JobProfileSkillSegmentModel, BodyViewModel>();

            CreateMap<JobProfileSkillSegmentDataModel, BodyDataViewModel>()
                .ForMember(d => d.Skills, s => s.MapFrom(a => a.Skills.Take(8)));

            CreateMap<Skills, SkillsViewModel>();

            CreateMap<OnetSkill, OnetSkillViewModel>();

            CreateMap<ContextualisedSkill, ContextualisedSkillViewModel>();

            CreateMap<Data.Models.Restriction, RestrictionViewModel>();

            CreateMap<JobProfileSkillSegmentModel, RefreshJobProfileSegmentServiceBusModel>()
                .ForMember(d => d.JobProfileId, s => s.MapFrom(a => a.DocumentId))
                .ForMember(d => d.Segment, s => s.MapFrom(a => JobProfileSkillSegmentDataModel.SegmentName));

            CreateMap<PatchRestrictionModel, Data.Models.Restriction>();

            CreateMap<PatchOnetSkillModel, OnetSkill>();

            CreateMap<PatchContextualisedModel, ContextualisedSkill>();

            CreateMap<PatchContextualisedModel, Skills>()
                .ForMember(d => d.OnetSkill, s => s.MapFrom(a => a.RelatedSkill))
                .ForMember(d => d.ContextualisedSkill, s => s.MapFrom(a => a));

            CreateMap<PatchContextualisedModel, OnetSkill>()
                .ForMember(d => d.Id, s => s.Ignore())
                .ForAllOtherMembers(s => s.MapFrom(a => a.RelatedSkill));

            CreateMap<RelatedSkill, OnetSkill>();
        }
    }
}