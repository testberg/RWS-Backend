using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationManagement.Api.Application
{
    public class FileContent
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public string Customer { get; set; }
    }
}