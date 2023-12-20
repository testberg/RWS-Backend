using System;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos
{
    public class TranslatorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double HourlyRate { get; set; }
        public string CreditCardNumber { get; set; }
    }
}
