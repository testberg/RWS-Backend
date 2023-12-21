using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.Dtos
{
    public interface ITranslatorManagementRepository
    {
        Task<List<TranslatorDto>> GetTranslatorsAsync();
        Task<TranslatorDto> GetTranslatorByNameAsync(string name);
        Task<Translator> GetTranslatorByIdAsync(Guid id);
        void CreateTranslatorAsync(Translator translatorDto);
        public void RemoveTranslator(Translator removeTranslator);
        Task<bool> SaveChangesAsync();
    }
}
