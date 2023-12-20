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
    public interface ITranslationFileReaderService
    {
        FileContent GetContnet(IFormFile file);
    }
}