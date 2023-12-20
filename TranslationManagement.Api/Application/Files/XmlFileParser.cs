using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public class XmlFileParser : IFileParser
    {
        public FileContent Parse(IFormFile file)
        {
            Console.WriteLine($"Parsing XML file: {file.FileName}");
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var xdoc = XDocument.Parse(reader.ReadToEnd());

                return new FileContent()
                {
                    Content = xdoc.Root.Element("Content").Value,
                    Customer = xdoc.Root.Element("Customer").Value.Trim()
                };
            }
        }
    }
}
