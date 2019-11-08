﻿using AutoMapper;
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
                .ForMember(d => d.Skills, s => s.MapFrom(a => a.Skills))
                .ForMember(d => d.RestrictionsAndRequirements, s => s.MapFrom(a => a))
                ;

            CreateMap<JobProfileSkillSegmentDataModel, RestrictionsAndRequirementsApiModel>()
                .ForMember(d => d.OtherRequirements, opt => opt.ConvertUsing(new HtmlStringFormatter()))
                .ForMember(d => d.RelatedRestrictions, s => s.MapFrom(a => a.Restrictions.Select(b => b.Description).ToList()))
                ;

            CreateMap<JobProfileSkillSegmentSkillDataModel, RelatedSkillsApiModel>()
                .ForMember(d => d.Description, s => s.MapFrom(a => a.ContextualisedDescription ?? a.StandardDescription))
                .ForMember(d => d.ONetAttributeType, s => s.Ignore())
                .ForMember(d => d.ONetRank, s => s.Ignore())
                .ForMember(d => d.ONetElementId, s => s.Ignore())
                ;
        }
    }
}