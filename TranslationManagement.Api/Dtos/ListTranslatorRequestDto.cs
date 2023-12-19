using System;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos
{
    public class ListTranslatorRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TranslatorStatus Status { get; set; }

        // no creditCard info should be exposed or rate.
        // so far no info security messures has been introudced 
    }
}
