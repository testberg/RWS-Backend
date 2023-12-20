using System;
using System.Threading.Tasks;
using External.ThirdParty.Services;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace TranslationManagement.Api.Application
{
    public interface INotificationClient
    {
        void Notify(Guid id);
    }
}
