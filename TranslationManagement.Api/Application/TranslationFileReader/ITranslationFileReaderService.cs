using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public interface ITranslationFileReaderService
    {
        FileContent GetContnet(IFormFile file);
    }
}