using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Entities
{
    public class TranslationJob
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public JobStatus Status { get; set; }
        public string OriginalContent { get; set; }
        public string TranslatedContent { get; set; }
        public double Price { get; set; }
        // nav property
        public Translator Translator { get; set; }
        public Guid TranslatorId { get; set; }
    }
}