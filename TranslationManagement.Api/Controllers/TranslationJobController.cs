using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Application;
using TranslationManagement.Api.Enums;
using TranslationManagement.Api.Dtos.TranslationJobs;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/[action]")]
    public class TranslationJobController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TranslationJobController> _logger;
        private readonly NotificationClient _notificationClient;
        private readonly TranslationFileReaderService _translationFileReader;
        private readonly Mapper _mapper;

        public TranslationJobController(AppDbContext context, ILogger<TranslationJobController> logger, NotificationClient notificationClient,
        TranslationFileReaderService translationFileReader,
        Mapper mapper
        )
        {
            _context = context;
            _logger = logger;
            _notificationClient = notificationClient;
            _translationFileReader = translationFileReader;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TranslationJobDto>>> GetJobs()
        {
            var jobs = await _context.TranslationJobs.ToListAsync();
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<ActionResult<TranslationJobDto>> CreateJobAsync(CreateTranslationJobDto job)
        {
            Guard.IsAssignableToType<CreateTranslationJobDto>(job);

            var jobObj = _mapper.Map<TranslationJob>(job);
            _context.TranslationJobs.Add(jobObj);

            bool success = await _context.SaveChangesAsync() > 0;

            if (!success)
            {
                return BadRequest("Failed to create job.");
            }


            _notificationClient.Notify(jobObj.Id);

            return _mapper.Map<TranslationJobDto>(jobObj);
        }

        [HttpPost]
        public ActionResult<TranslationJobDto> CreateJobWithFile(IFormFile file, string customer)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            var content = _translationFileReader.GetContnet(file);

            if (content == null)
            {
                return BadRequest("Unsupported file format.");
            }

            var newJob = new CreateTranslationJobDto()
            {
                CustomerName = customer != null ? customer : content.Customer,
                OriginalContent = content.Content
            };

            return CreatedAtAction(nameof(CreateJobAsync), newJob);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateJobStatus(Guid jobId, Guid translatorId, string newStatus = "")
        {
            Guard.IsAssignableToType<Guid>(jobId);
            Guard.IsAssignableToType<Guid>(translatorId);
            Guard.IsAssignableToType<JobStatus>(newStatus);

            _logger.LogInformation($"Job status update request received: {newStatus} for job {jobId} by translator {translatorId}");

            var job = _context.TranslationJobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return NotFound("Translator not found.");
            }

            Enum.TryParse(newStatus, out JobStatus status);

            if ((int)status >= (int)job.Status)
            {
                return BadRequest("Translator status updated successfully.");
            }

            job.Status = status;
            try
            {
                var result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error");
            }

            return Ok("Job status updated.");
        }


        [HttpPut]
        public async Task<IActionResult> UpdateJobTranslator(Guid jobId, Guid translatorId)
        {

            Guard.IsAssignableToType<Guid>(jobId);
            Guard.IsAssignableToType<Guid>(translatorId);

            _logger.LogInformation($"Set translator {translatorId} for job {jobId}");

            var translator = _context.Translators.FirstOrDefault(j => j.Id == translatorId);

            if (translator.Status != TranslatorStatus.Certified)
            {
                return BadRequest($"Translator must be {TranslatorStatus.Certified}.");
            }

            var job = _context.TranslationJobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return NotFound("Translator not found.");
            }

            try
            {
                var result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error");
            }

            return Ok("Job status updated.");
        }
    }
}