using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos
{
    public class CreateTranslatorRequestDto
    {
        public string Name { get; set; }
        public TranslatorStatus Status = TranslatorStatus.Applicant;
        public double HourlyRate { get; set; }
        public string CreditCardNumber { get; set; }
    }
}
