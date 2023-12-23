using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos
{
    public class CreateTranslationJobDto
    {
        private const double PRICE_PER_CHARACHTER = 0.01;
        private string _customer;
        private string _originalContent;

        public CreateTranslationJobDto()
        {
            Status = JobStatus.New.ToString();
            TranslatedContent = "";
        }

        [Required(ErrorMessage = "CustomerName cannot be empty.")]
        public string CustomerName
        {
            get { return _customer; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("CustomerName cannot be empty or whitespace.");
                }
                _customer = value;
            }
        }

        [Required(ErrorMessage = "OriginalContent cannot be empty.")]
        public string OriginalContent
        {
            get { return _originalContent; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("OriginalContent cannot be empty or whitespace.");
                }
                _originalContent = value;
                Price = PRICE_PER_CHARACHTER * value.Length;
            }
        }
        public double Price { get; private set; }
        public string Status { get; private set; }
        public string TranslatedContent { get; private set; }
    }
}
