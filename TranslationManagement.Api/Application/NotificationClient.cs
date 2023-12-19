using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using External.ThirdParty.Services;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace TranslationManagement.Api.Application
{
    public class NotificationClient
    {
        private readonly ILogger<NotificationClient> _logger;
        private readonly INotificationService _notificationService;
        public NotificationClient(ILogger<NotificationClient> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }
        public async Task<bool> Notify(int id)
        {
            AsyncRetryPolicy RetryPolicy = Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(0));

            bool notificationSent = await RetryPolicy
            .ExecuteAsync(() => _notificationService.SendNotification("Job created: " + id));

            return notificationSent;
        }
    }
}