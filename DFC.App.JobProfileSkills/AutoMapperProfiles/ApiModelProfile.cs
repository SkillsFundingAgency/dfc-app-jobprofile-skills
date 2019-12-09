using AutoMapper;
using DFC.App.JobProfileSkills.ApiModels;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.HtmlToDataTranslator.Contracts;
using DFC.HtmlToDataTranslator.Services;
using DFC.HtmlToDataTranslator.ValueConverters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfileSkills.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ApiModelProfile : Profile
    {
        private readonly IHtmlToDataTranslator htmlTranslator;

        public ApiModelProfile()
        {
            htmlTranslator = new HtmlAgilityPackDataTranslator();
            var htmlToStringValueConverter = new HtmlToStringValueConverter(htmlTranslator);

            CreateMap<JobProfileSkillSegmentDataModel, WhatItTakesApiModel>()
                .ForMember(d => d.DigitalSkillsLevel, s => s.MapFrom(a => a.DigitalSkill))
                .ForMember(d => d.RestrictionsAndRequirements, s => s.MapFrom(a => a))
                ;

            CreateMap<JobProfileSkillSegmentDataModel, RestrictionsAndRequirementsApiModel>()
                .ForMember(d => d.OtherRequirements, opt => opt.ConvertUsing(htmlToStringValueConverter))
                .ForMember(d => d.RelatedRestrictions, s => s.MapFrom(ConvertToList))
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

        private List<string> ConvertToList(JobProfileSkillSegmentDataModel source, RestrictionsAndRequirementsApiModel destination)
        {
            return ConvertToList(source.Restrictions.Select(x => x.Description).ToList());
        }

        private List<string> ConvertToList(List<string> source)
        {
            var result = new List<string>();
            foreach (var item in source)
            {
                result.AddRange(htmlTranslator.Translate(item));
            }

            return result;
        }
    }
}