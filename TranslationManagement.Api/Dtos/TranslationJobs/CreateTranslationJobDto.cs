using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationManagement.Api.Enums;

namespace TranslationManagement.Api.Dtos.TranslationJobs
{
    public class CreateTranslationJobDto
    {
        private static double PricePerCharacter = 0.01;
        string _originalContent = "";
        double _price = 0;
        public string CustomerName { get; set; }
        public JobStatus Status = JobStatus.New;
        public string OriginalContent
        {
            get { return _originalContent; }
            set
            {
                _originalContent = value;
            }
        }
        public string TranslatedContent = "";
        public double Price
        {
            get { return _price; }
            set
            {
                if (_price == 0)
                {
                    _price = _originalContent.Length * PricePerCharacter;
                }
                else
                {

                    _price = value;
                }
            }
        }
    }
}