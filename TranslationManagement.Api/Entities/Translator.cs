using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Entities
{
    public class Translator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HourlyRate { get; set; }
        public TranslatorStatus Status { get; set; }
        public string CreditCardNumber { get; set; }
    }
}