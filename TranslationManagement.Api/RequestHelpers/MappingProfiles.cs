using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TranslationManagement.Api.Dtos;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TranslationJob, TranslationJobDto>();
            CreateMap<CreateTranslationJobDto, TranslationJob>();

            CreateMap<Translator, TranslatorDto>();
            CreateMap<CreateTranslatorDto, Translator>();
        }

    }
}