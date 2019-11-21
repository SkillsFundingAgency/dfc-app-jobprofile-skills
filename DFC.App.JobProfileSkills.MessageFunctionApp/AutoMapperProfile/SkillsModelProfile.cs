using AutoMapper;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels;
using DFC.App.JobProfileSkills.Data.ServiceBusModels.PatchModels;
using System.Linq;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.AutoMapperProfile
{
    public class SkillsModelProfile : Profile
    {
        public SkillsModelProfile()
        {
            CreateMap<JobProfileMessage, JobProfileSkillSegmentModel>()
                .ForMember(d => d.Data, s => s.MapFrom(a => a))
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.JobProfileId))
                .ForMember(d => d.Etag, s => s.Ignore());

            CreateMap<JobProfileMessage, JobProfileSkillSegmentDataModel>()
                .ForMember(d => d.Restrictions, s => s.MapFrom(a => a.Restrictions))
                .ForMember(d => d.DigitalSkill, s => s.MapFrom(a => a.DigitalSkillsLevel))
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.LastModified))
                .ForMember(d => d.Skills, s => s.MapFrom(a => a.SocSkillsMatrixData));

            CreateMap<Data.ServiceBusModels.Restriction, Data.Models.Restriction>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Info));

            CreateMap<SocSkillsMatrix, Skills>()
                .ForMember(d => d.ContextualisedSkill, s => s.MapFrom(a => a))
                .ForMember(d => d.OnetSkill, s => s.MapFrom(a => a.RelatedSkill.FirstOrDefault()));

            CreateMap<SocSkillsMatrix, ContextualisedSkill>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Contextualised))
                .ForMember(d => d.OriginalRank, s => s.MapFrom(a => a.Rank.GetValueOrDefault()))
                .ForMember(d => d.ONetRank, s => s.MapFrom(a => a.ONetRank.GetValueOrDefault()));

            CreateMap<RelatedSkill, OnetSkill>();

            CreateMap<PatchDigitalSkillsLevelServiceBusModel, PatchDigitalSkillModel>();

            CreateMap<PatchOnetSkillServiceBusModel, PatchOnetSkillModel>();

            CreateMap<PatchRestrictionsServiceBusModel, PatchRestrictionModel>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Info));

            CreateMap<PatchSkillsMatrixServiceBusModel, PatchContextualisedModel>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Contextualised))
                .ForMember(d => d.OriginalRank, s => s.MapFrom(a => a.Rank));
        }
    }
}