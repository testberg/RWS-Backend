using System;
using System.Threading.Tasks;
using External.ThirdParty.Services;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace TranslationManagement.Api.Application
{
    public class NotificationClient : INotificationClient
    {
        private readonly ILogger<NotificationClient> _logger;
        private readonly INotificationService _notificationService;
        private readonly AsyncRetryPolicy _retryPolicy;

        public NotificationClient(ILogger<NotificationClient> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;

            _retryPolicy = Policy
                .Handle<Exception>()
                .RetryForeverAsync();
        }

        public async void Notify(Guid id)
        {
            try
            {
                bool notificationSent = await _retryPolicy.ExecuteAsync(async () =>
                {
                    return await _notificationService.SendNotification($"Job created: {id}");
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification for job {id}: {ex.Message}");
            }
        }
    }
}
