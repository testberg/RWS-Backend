using System;
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
        private readonly AsyncRetryPolicy _retryPolicy;

        public NotificationClient(ILogger<NotificationClient> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    4,
                    attempt => TimeSpan.FromSeconds(1),
                    (exception, timeSpan, attempt, context) =>
                    {
                        _logger.LogWarning($"Retry attempt {attempt} failed after {timeSpan.TotalSeconds} seconds. Exception: {exception.Message}");
                    });
        }

        public async Task<bool> Notify(int id)
        {
            try
            {
                bool notificationSent = await _retryPolicy.ExecuteAsync(async () =>
                {
                    return await _notificationService.SendNotification($"Job created: {id}");
                });

                return notificationSent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification for job {id}: {ex.Message}");
                return false;
            }
        }
    }
}
