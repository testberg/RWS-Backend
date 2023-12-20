using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TranslationManagement.Api.Application
{
    public interface IFileParser
    {
        FileContent Parse(IFormFile file);
    }
}