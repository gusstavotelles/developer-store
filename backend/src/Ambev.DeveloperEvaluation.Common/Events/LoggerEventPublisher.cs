using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Common.Events
{
    public class LoggerEventPublisher : IEventPublisher
    {
        private readonly ILogger<LoggerEventPublisher> _logger;

        public LoggerEventPublisher(ILogger<LoggerEventPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(object @event, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = JsonSerializer.Serialize(@event);
                _logger.LogInformation("Event published: {EventType} {Payload}", @event.GetType().Name, payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize event for publishing");
            }

            return Task.CompletedTask;
        }
    }
}
