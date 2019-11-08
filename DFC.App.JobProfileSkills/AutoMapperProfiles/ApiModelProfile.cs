using AutoMapper;
using DFC.App.JobProfileSkills.ApiModels;
using DFC.App.JobProfileSkills.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.AutoMapperProfiles
{
    public class HtmlStringFormatter : IValueConverter<string, List<string>>
    {
        public List<string> Convert(string sourceMember, ResolutionContext context)
        {
            return new List<string> { sourceMember };
        }
    }

    public class ApiModelProfile : Profile
    {
        public ApiModelProfile()
        {
            CreateMap<JobProfileSkillSegmentDataModel, WhatItTakesApiModel>()
                .ForMember(d => d.DigitalSkillsLevel, s => s.MapFrom(a => a.DigitalSkill))
                .ForMember(d => d.RestrictionsAndRequirements, s => s.MapFrom(a => a))
                ;

            CreateMap<JobProfileSkillSegmentDataModel, RestrictionsAndRequirementsApiModel>()
                .ForMember(d => d.OtherRequirements, opt => opt.ConvertUsing(new HtmlStringFormatter()))
                .ForMember(d => d.RelatedRestrictions, s => s.MapFrom(a => a.Restrictions.Select(b => b.Description).ToList()))
                ;

            CreateMap<Skills, RelatedSkillsApiModel>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.ContextualisedSkill.Description ?? a.OnetSkill.Description))
                .ForMember(d => d.ONetAttributeType, s => s.MapFrom(a => a.ContextualisedSkill.ONetAttributeType))
                .ForMember(d => d.ONetRank, s => s.MapFrom(a => a.ContextualisedSkill.ONetRank))
                .ForMember(d => d.ONetElementId, s => s.MapFrom(a => a.OnetSkill.ONetElementId))
                ;
        }
    }
}