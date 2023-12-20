using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public class TxtFileParser : IFileParser
    {
        public FileContent Parse(IFormFile file)
        {
            Console.WriteLine($"Parsing TXT file: {file.FileName}");
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return new FileContent()
                {
                    Content = reader.ReadToEnd().Trim(),
                    Customer = ""
                };
            }
        }
    }
}
