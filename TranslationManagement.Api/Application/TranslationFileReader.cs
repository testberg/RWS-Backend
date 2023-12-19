using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using TranslationManagement.Api.Dtos.TranslationJobs;

namespace TranslationManagement.Api.Application
{
    public class TranslationFileReader
    {
        public CreateTranslationJobDto GetContnet(IFormFile file, string customer)
        {
            var job = new CreateTranslationJobDto();
            var reader = new StreamReader(file.OpenReadStream());

            /**
            ** TODO: split it into more Parsing classes and interface files.. it is ok for now
            **/

            if (file.FileName.EndsWith(".txt"))
            {
                job.OriginalContent = reader.ReadToEnd();
                job.CustomerName = customer;
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                var xdoc = XDocument.Parse(reader.ReadToEnd());
                job.OriginalContent = xdoc.Root.Element("Content").Value;
                job.CustomerName = xdoc.Root.Element("Customer").Value.Trim();
            }
            else
            {
                return null;
            }

            return job;
        }
    }
}