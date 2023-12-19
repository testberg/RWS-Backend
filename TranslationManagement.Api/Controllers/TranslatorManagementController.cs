using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        public TranslatorManagementController(AppDbContext context, ILogger<TranslatorManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetTranslators()
        {
            var translators = _context.Translators.ToArray();
            return Ok(translators);
        }

        [HttpGet]
        public IActionResult GetTranslatorsByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var translators = _context.Translators.Where(t => t.Name == name).ToArray();
            return Ok(translators);
        }

        [HttpPost]
        public IActionResult AddTranslator(Translator translator)
        {
            if (translator == null)
            {
                return BadRequest("Translator object is null.");
            }

            _context.Translators.Add(translator);
            bool success = _context.SaveChanges() > 0;

            if (success)
            {
                return Ok("Translator added successfully.");
            }

            return BadRequest("Failed to add translator.");
        }

        [HttpPut]
        public IActionResult UpdateTranslatorStatus(int translatorId, string newStatus = "")
        {
            _logger.LogInformation($"User status update request: {newStatus} for user {translatorId}");

            if (string.IsNullOrEmpty(newStatus))
            {
                return BadRequest("New status is required.");
            }

            if (!Enum.TryParse(newStatus, out TranslatorStatus status))
            {
                return BadRequest("Unknown status.");
            }

            var translator = _context.Translators.SingleOrDefault(t => t.Id == translatorId);

            if (translator != null)
            {
                translator.Status = status;
                _context.SaveChanges();
                return Ok("Translator status updated successfully.");
            }

            return NotFound("Translator not found.");
        }
    }
}
