using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Dtos.Translator;
using TranslationManagement.Api.Entities;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Controlers
{
    [ApiController]
    [Route("api/TranslatorsManagement/[action]")]
    public class TranslatorManagementController : ControllerBase
    {
        private readonly ILogger<TranslatorManagementController> _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TranslatorManagementController(AppDbContext context, ILogger<TranslatorManagementController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TranslatorDto>>> GetTranslators()
        {
            var translators = await _context.Translators
            .ToListAsync();

            return Ok(_mapper.Map<List<TranslatorDto>>(translators));
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<TranslatorDto>> GetTranslatorsByName(string name)
        {
            Guard.IsNotNullOrEmpty(name);
            var translator = await _context.Translators
                .FirstOrDefaultAsync(t => t.Name == name);

            if (translator == null) return NotFound($"Translator {name} not found!");

            return Ok(_mapper.Map<TranslatorDto>(translator));
        }

        [HttpPost]
        public async Task<ActionResult<TranslatorDto>> CreateTranslator(CreateTranslatorDto translatorDto)
        {
            Guard.IsAssignableToType<CreateTranslatorDto>(translatorDto);

            var translator = _mapper.Map<Translator>(translatorDto);

            _context.Translators.Add(translator);
            try
            {
                var result = await _context.SaveChangesAsync() > 0;

                if (!result)
                {
                    return BadRequest("Failed to add translator.");
                }

                return CreatedAtAction(nameof(GetTranslatorsByName),
               new { translator.Name }, _mapper.Map<TranslatorDto>(translator));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTranslatorStatus(Guid id, string newStatus)
        {
            Guard.IsAssignableToType<Guid>(id);
            Guard.IsAssignableToType<TranslatorStatus>(newStatus);

            _logger.LogInformation($"User status update request: {newStatus} for translator {id}");

            var translator = _context.Translators.FirstOrDefault(t => t.Id == id);

            Enum.TryParse(newStatus, out TranslatorStatus status);

            translator.Status = status;
            try
            {
                var result = await _context.SaveChangesAsync() > 0;

                if (result)
                {
                    return Ok("Translator status updated successfully.");

                }
                return NotFound("Translator not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error");
            }
        }
    }
}
