using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using External.ThirdParty.Services;
using TranslationManagement.Api.Application;
using TranslationManagement.Api.Entities;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/[action]")]
    public class TranslationJobController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TranslationJobController> _logger;
        private readonly NotificationClient _notificationClient;
        private const double PricePerCharacter = 0.01; // It should be a value in the database

        public TranslationJobController(AppDbContext context, ILogger<TranslationJobController> logger, NotificationClient notificationClient)
        {
            _context = context;
            _logger = logger;
            _notificationClient = notificationClient;
        }

        [HttpGet]
        public IActionResult GetJobs()
        {
            var jobs = _context.TranslationJobs.ToArray();
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobAsync(TranslationJob job)
        {
            job.Status = JobStatus.New;
            SetPrice(job);
            _context.TranslationJobs.Add(job);
            bool success = await _context.SaveChangesAsync() > 0;

            if (!success)
            {
                return BadRequest("Failed to create job.");
            }

            try
            {
                bool notificationResult = await _notificationClient.Notify(job.Id);
                return Ok(notificationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send new job notification: {ex.Message}");
                return StatusCode(500, "Failed to send notification.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobWithFile(IFormFile file, string customer)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            var reader = new StreamReader(file.OpenReadStream());
            string content;

            if (file.FileName.EndsWith(".txt"))
            {
                content = reader.ReadToEnd();
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                var xdoc = XDocument.Parse(reader.ReadToEnd());
                content = xdoc.Root.Element("Content").Value;
                customer = xdoc.Root.Element("Customer").Value.Trim();
            }
            else
            {
                return BadRequest("Unsupported file format.");
            }

            var newJob = new TranslationJob()
            {
                OriginalContent = content,
                TranslatedContent = "",
                CustomerName = customer,
            };

            SetPrice(newJob);

            return await CreateJobAsync(newJob);
        }

        [HttpPut]
        public IActionResult UpdateJobStatus(int jobId, int translatorId, string newStatus = "")
        {
            _logger.LogInformation($"Job status update request received: {newStatus} for job {jobId} by translator {translatorId}");
            JobStatus status;

            if (Enum.TryParse(newStatus, out status))
            {
                var job = _context.TranslationJobs.SingleOrDefault(j => j.Id == jobId);

                if (job != null && (int)status >= (int)job.Status)
                {
                    job.Status = status;
                    _context.SaveChanges();
                    return Ok("Job status updated.");
                }
            }

            return BadRequest("Invalid status or job not found.");
        }

        private void SetPrice(TranslationJob job)
        {
            job.Price = job.OriginalContent.Length * PricePerCharacter;
        }
    }
}