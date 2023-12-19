using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using External.ThirdParty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TranslationManagement.Api.Application;
using TranslationManagement.Api.Controlers;
using TranslationManagement.Api.Entities;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/[action]")]
    public class TranslationJobController : ControllerBase
    {
        private AppDbContext _context;
        private readonly ILogger<TranslatorManagementController> _logger;
        private readonly NotificationClient _notificationClient;
        const double PricePerCharacter = 0.01; // it should be a value in DB
        public TranslationJobController(IServiceScopeFactory scopeFactory, ILogger<TranslatorManagementController> logger, NotificationClient notificationClient)
        {
            _context = scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            _logger = logger;
            _notificationClient = notificationClient;
        }

        [HttpGet]
        public TranslationJob[] GetJobs()
        {
            return _context.TranslationJobs.ToArray();
        }

        [HttpPost]
        public async Task<bool> CreateJobAsync(TranslationJob job)
        {
            job.Status = JobStatus.New;
            SetPrice(job);
            _context.TranslationJobs.Add(job);
            bool success = _context.SaveChanges() > 0;

            if (!success) return false;

            try
            {
                return await _notificationClient.Notify(job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"New job notification sent {ex.Message}");
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> CreateJobWithFile(IFormFile file, string customer)
        {
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
                throw new NotSupportedException("unsupported file");
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

        [HttpPost]
        public string UpdateJobStatus(int jobId, int translatorId, string newStatus = "")
        {
            _logger.LogInformation("Job status update request received: " + newStatus + " for job " + jobId.ToString() + " by translator " + translatorId);
            JobStatus status;
            bool parsed = Enum.TryParse(newStatus, out status);
            var job = _context.TranslationJobs.Single(j => j.Id == jobId);

            if (!parsed || (int)status < (int)job.Status)
            {
                return "invalid status";
            }

            job.Status = status;
            _context.SaveChanges();
            return "updated";
        }

        private void SetPrice(TranslationJob job)
        {
            job.Price = job.OriginalContent.Length * PricePerCharacter;
        }
    }
}