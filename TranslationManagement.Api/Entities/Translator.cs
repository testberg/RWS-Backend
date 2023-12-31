using System;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Entities
{

    public class Translator
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double HourlyRate { get; set; }
        public TranslatorStatus Status { get; set; }
        public string CreditCardNumber { get; set; }
    }
}