using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public class FileParser
    {
        private IFileParser _fileParser;

        public void SetFileParser(IFileParser fileParser)
        {
            _fileParser = fileParser;
        }

        public FileContent ParseFile(IFormFile file)
        {
            if (_fileParser == null)
            {
                throw new InvalidOperationException("File parser strategy not set.");
            }

            return _fileParser.Parse(file);
        }
    }
}