using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos.TranslationJobs
{
    public class TranslationJobDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string OriginalContent { get; set; }
        public string TranslatedContent { get; set; }
        public double Price { get; set; }
        public Guid? TranslatorId { get; set; }
    }
}