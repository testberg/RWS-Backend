using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TranslationManagement.Api.Dtos.TranslationJobs;
using TranslationManagement.Api.Dtos.Translator;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TranslationJob, TranslationJobDto>().IncludeMembers(x => x.Translator);
            CreateMap<CreateTranslationJobDto, TranslationJob>();//.ForMember(x => x.Translator, y => y.MapFrom(y => y));

            CreateMap<Translator, TranslatorDto>();
            CreateMap<CreateTranslationJobDto, Translator>();
        }

    }
}