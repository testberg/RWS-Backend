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
    public class TranslationFileReaderService : ITranslationFileReaderService
    {
        public FileContent GetContnet(IFormFile file)
        {
            var job = new CreateTranslationJobDto();
            var fileParserFactory = new FileParserFactory();

            var fileParser = fileParserFactory.CreateFileParser(file.FileName);

            var contextParser = new FileParser();
            contextParser.SetFileParser(fileParser);

            return contextParser.ParseFile(file);
        }
    }
}