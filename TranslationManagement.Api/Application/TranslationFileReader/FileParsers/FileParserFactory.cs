using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationManagement.Api.Application
{
    public class FileParserFactory
    {
        public IFileParser CreateFileParser(string filePath)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".xml":
                    return new XmlFileParser();
                case ".txt":
                    return new TxtFileParser();
                default:
                    throw new NotSupportedException($"File type not supported: {fileExtension}");
            }
        }
    }
}