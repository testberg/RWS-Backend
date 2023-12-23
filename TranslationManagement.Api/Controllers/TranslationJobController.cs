using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Application;
using TranslationManagement.Api.Enums;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper;
using TranslationManagement.Api.Entities;
using TranslationManagement.Api.Dtos;
using System.IO;
using AutoMapper.QueryableExtensions;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/[action]")]
    public class TranslationJobController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TranslationJobController> _logger;
        private readonly INotificationClient _notificationClient;
        private readonly ITranslationFileReaderService _translationFileReader;
        private readonly IMapper _mapper;
        public TranslationJobController(AppDbContext context,
        ILogger<TranslationJobController> logger,
        INotificationClient notificationClient,
        ITranslationFileReaderService translationFileReader,
        IMapper mapper
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
            var jobsDtos = await _context.TranslationJobs
            .ProjectTo<TranslationJobDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

            return Ok(jobsDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TranslationJobDto>> CreateJobAsync(CreateTranslationJobDto job)
        {
            return await CreateJob(job);
        }

        [HttpPost]
        public async Task<ActionResult<TranslationJobDto>> CreateJobWithFile(IFormFile file, string customer)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }

            if (file.Length > 10485760) // 10 MB
            {
                return BadRequest("File size exceeds the limit.");
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

            return await CreateJob(newJob);
        }

        [HttpPatch("{jobId}/{translatorId}")]
        public async Task<IActionResult> UpdateJobStatus(Guid jobId, Guid translatorId, string newStatus = "")
        {
            Enum.TryParse(newStatus, out JobStatus status);

            Guard.IsAssignableToType(jobId, typeof(Guid));
            Guard.IsAssignableToType(translatorId, typeof(Guid));
            Guard.IsAssignableToType(status, typeof(JobStatus));

            _logger.LogInformation($"Job status update request received: {newStatus} for job {jobId} by translator {translatorId}");

            var job = _context.TranslationJobs.FirstOrDefault(j => j.Id == jobId && j.TranslatorId == translatorId);

            if (Math.Abs((int)status - (int)job.Status) != 1)
            {
                return BadRequest("Invalid Status value");
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

            return Ok("Job's status updated.");
        }


        [HttpPatch("{jobId}")]
        public async Task<IActionResult> UpdateJobTranslator(Guid jobId, Guid translatorId)
        {
            Guard.IsAssignableToType(jobId, typeof(Guid));
            Guard.IsAssignableToType(translatorId, typeof(Guid));

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

            job.TranslatorId = translatorId;

            try
            {
                var result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error");
            }

            return Ok("Job's translator updated.");
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJob(Guid jobId, UpdateTranslationJobDto updateTranslationJobDto)
        {
            Guard.IsAssignableToType(jobId, typeof(Guid));
            Guard.IsNotNullOrEmpty(updateTranslationJobDto.TranslatedContent);

            _logger.LogInformation($"Update job {jobId}");

            var job = _context.TranslationJobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return NotFound("Translator not found.");
            }

            job.TranslatedContent = updateTranslationJobDto.TranslatedContent;

            try
            {
                var result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            return Ok("Job updated.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteJob(Guid id)
        {
            Guard.IsAssignableToType(id, typeof(Guid));

            var job = await _context.TranslationJobs.FindAsync(id);

            if (job == null) return NotFound();

            _context.TranslationJobs.Remove(job);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return StatusCode(500, new { Message = "Delete failed due to an error" });

            return NoContent();
        }

        async Task<ActionResult<TranslationJobDto>> CreateJob(CreateTranslationJobDto job)
        {
            var jobObj = _mapper.Map<TranslationJob>(job);

            _context.TranslationJobs.Add(jobObj);

            bool success = await _context.SaveChangesAsync() > 0;

            if (!success)
            {
                return StatusCode(500, new { Message = "Insert failed due to an error" });
            }

            _notificationClient.Notify(jobObj.Id);

            return Ok(_mapper.Map<TranslationJobDto>(jobObj));
        }
    }
}