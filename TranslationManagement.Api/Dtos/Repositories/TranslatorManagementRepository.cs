using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.Dtos
{
    public class TranslatorManagementRepository : ITranslatorManagementRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TranslatorManagementRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async void CreateTranslatorAsync(Translator translatorDto)
        {
            await _context.Translators
            .AddAsync(translatorDto);
        }

        public async Task<List<TranslatorDto>> GetTranslatorsAsync()
        {
            return await _context.Translators
            .ProjectTo<TranslatorDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public async Task<TranslatorDto> GetTranslatorByNameAsync(string name)
        {
            return await _context.Translators
            .ProjectTo<TranslatorDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<Translator> GetTranslatorByIdAsync(Guid id)
        {
            return await _context.Translators
            .FirstOrDefaultAsync(t => t.Id == id);
        }

        public void RemoveTranslator(Translator removeTranslator)
        {
            _context.Translators.Remove(removeTranslator);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
