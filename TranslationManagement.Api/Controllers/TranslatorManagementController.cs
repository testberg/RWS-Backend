using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Dtos;
using TranslationManagement.Api.Entities;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Controlers
{
    [ApiController]
    [Route("api/TranslatorsManagement/[action]")]
    public class TranslatorManagementController : ControllerBase
    {
        private readonly ILogger<TranslatorManagementController> _logger;
        private readonly ITranslatorManagementRepository _repo;
        private readonly IMapper _mapper;

        public TranslatorManagementController(ITranslatorManagementRepository repo, ILogger<TranslatorManagementController> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TranslatorDto>>> GetTranslators()
        {
            return Ok(await _repo.GetTranslatorsAsync());
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<TranslatorDto>> GetTranslatorByNameAsync(string name)
        {
            Guard.IsNotNullOrEmpty(name);
            var translator = await _repo.GetTranslatorByNameAsync(name);

            if (translator == null) return NotFound($"Translator {name} not found!");

            var translatorDto = _mapper.Map<TranslatorDto>(translator);

            return Ok(translatorDto);
        }

        [HttpPost]
        public async Task<ActionResult<TranslatorDto>> CreateTranslator([FromBody] CreateTranslatorDto translatorDto)
        {
            Guard.IsAssignableToType<CreateTranslatorDto>(translatorDto);
            var translator = _mapper.Map<Translator>(translatorDto);

            _repo.CreateTranslatorAsync(translator);

            try
            {
                var result = await _repo.SaveChangesAsync();

                if (!result)
                {
                    return BadRequest("Failed to add translator.");
                }

                var newTranslator = await _repo.GetTranslatorByIdAsync(translator.Id);

                var newTranslatorDto = _mapper.Map<TranslatorDto>(newTranslator);

                return Ok(newTranslatorDto);
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
            Enum.TryParse(newStatus, out TranslatorStatus status);
            Guard.IsAssignableToType<TranslatorStatus>(status);

            _logger.LogInformation($"User status update request: {newStatus} for translator {id}");

            var translator = await _repo.GetTranslatorByIdAsync(id);

            translator.Status = status;
            try
            {
                var result = await _repo.SaveChangesAsync();

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTranslator(Guid id)
        {
            Guard.IsAssignableToType(id, typeof(Guid));

            var translator = await _repo.GetTranslatorByIdAsync(id);

            if (translator == null) return NotFound();

            _repo.RemoveTranslator(translator);

            var result = await _repo.SaveChangesAsync();

            if (!result) return StatusCode(500, new { Message = "Delete failed due to an error" });

            return NoContent();
        }
    }
}