using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public class TranslationFileReaderService : ITranslationFileReaderService
    {
        public FileContent GetContnet(IFormFile file)
        {
            var fileParserFactory = new FileParserFactory();

            var fileParser = fileParserFactory.CreateFileParser(file.FileName);

            var contextParser = new FileParser();
            contextParser.SetFileParser(fileParser);

            return contextParser.ParseFile(file);
        }
    }
}