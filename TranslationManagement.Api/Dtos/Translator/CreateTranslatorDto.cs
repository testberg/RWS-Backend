using System.ComponentModel.DataAnnotations;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos
{
    public class CreateTranslatorDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public double HourlyRate { get; set; }
        [Required]
        public string CreditCardNumber { get; set; }
    }
}
