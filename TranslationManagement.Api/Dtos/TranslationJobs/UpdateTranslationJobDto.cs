using System;
using System.ComponentModel.DataAnnotations;

namespace TranslationManagement.Api.Dtos
{
    public class UpdateTranslationJobDto
    {
        public string TranslatedContent { get; set; }
    }
}