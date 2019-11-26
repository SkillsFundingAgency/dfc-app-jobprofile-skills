using AutoMapper;
using DFC.App.JobProfileSkills.ApiModels;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.HtmlToDataTranslator.Services;
using DFC.HtmlToDataTranslator.ValueConverters;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfileSkills.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ApiModelProfile : Profile
    {
        public ApiModelProfile()
        {
            var htmlToStringValueConverter = new HtmlToStringValueConverter(new HtmlAgilityPackDataTranslator());

            CreateMap<JobProfileSkillSegmentDataModel, WhatItTakesApiModel>()
                .ForMember(d => d.DigitalSkillsLevel, s => s.MapFrom(a => a.DigitalSkill))
                .ForMember(d => d.RestrictionsAndRequirements, s => s.MapFrom(a => a))
                ;

            CreateMap<JobProfileSkillSegmentDataModel, RestrictionsAndRequirementsApiModel>()
                .ForMember(d => d.OtherRequirements, opt => opt.ConvertUsing(htmlToStringValueConverter))
                .ForMember(d => d.RelatedRestrictions, s => s.MapFrom(a => a.Restrictions.Select(b => b.Description).ToList()))
                ;

            CreateMap<Skills, RelatedSkillsApiModel>()
                .ForMember(d => d.Description, s => s.MapFrom(
                    a => !string.IsNullOrWhiteSpace(a.ContextualisedSkill.Description)
                        ? a.ContextualisedSkill.Description
                        : a.OnetSkill.Description))
                .ForMember(d => d.ONetAttributeType, s => s.MapFrom(a => a.ContextualisedSkill.ONetAttributeType))
                .ForMember(d => d.ONetRank, s => s.MapFrom(a => a.ContextualisedSkill.ONetRank))
                .ForMember(d => d.ONetElementId, s => s.MapFrom(a => a.OnetSkill.ONetElementId))
                ;
        }
    }
}