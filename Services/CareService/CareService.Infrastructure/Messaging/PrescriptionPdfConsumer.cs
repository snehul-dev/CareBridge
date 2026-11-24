
using System.Text;
using System.Text.Json;
using CareBridge.Contracts.Messaging;
using CareService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CareService.Infrastructure.Messaging
{
    public class PrescriptionPdfConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PrescriptionPdfConsumer> _logger;

        public PrescriptionPdfConsumer(
            IConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<PrescriptionPdfConsumer> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await using var channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await PrescriptionQueueTopology.DeclareAsync(
                channel, stoppingToken);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                // RabbitMQ message memory should be copied
                // before any asynchronous processing.
                var body = args.Body.ToArray();

                try
                {
                    var message =
                        JsonSerializer.Deserialize<PrescriptionPdfRequested>(
                            body);

                    if (message is null ||
                        message.PrescriptionId == Guid.Empty)
                    {
                        throw new JsonException(
                            "Invalid prescription message.");
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var pdfService = scope.ServiceProvider
                        .GetRequiredService<IPrescriptionPdfService>();

                    await pdfService.GenerateInternalAsync(
                        message.PrescriptionId,
                        stoppingToken);

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);

                    _logger.LogInformation(
                        "Prescription PDF generated successfully: {Id}",
                        message.PrescriptionId);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    // Shutdown: RabbitMQ will requeue
                    // the unacknowledged message when
                    // the connection/channel closes.
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Prescription PDF processing failed.");

                    try
                    {
                        await HandleFailureAsync(
                            channel,
                            args,
                            body,
                            ex is JsonException,
                            stoppingToken);
                    }
                    catch (Exception routingEx)
                    {
                        _logger.LogError(
                            routingEx,
                            "Failed to route prescription message.");

                        // Do not acknowledge a message whose
                        // retry routing was unsuccessful.
                        // A channel/connection failure can also
                        // cause RabbitMQ to redeliver it.
                        if (channel.IsOpen)
                        {
                            await channel.BasicNackAsync(
                                deliveryTag: args.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken);
                        }
                    }
                }
            };

            await channel.BasicConsumeAsync(
                queue: PrescriptionQueueTopology.MainQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(
                Timeout.InfiniteTimeSpan,
                stoppingToken);
        }

        private async Task HandleFailureAsync(
            IChannel channel,
            BasicDeliverEventArgs args,
            byte[] body,
            bool permanentFailure,
            CancellationToken cancellationToken)
        {
            int retryCount = GetRetryCount(args.BasicProperties.Headers);

            string destination =
                permanentFailure ||
                retryCount >= PrescriptionQueueTopology.MaxRetries
                    ? PrescriptionQueueTopology.FailedQueue
                    : PrescriptionQueueTopology.RetryQueue;

            var headers = new Dictionary<string, object?>();

            if (args.BasicProperties.Headers != null)
            {
                foreach (var header in args.BasicProperties.Headers)
                {
                    headers[header.Key] = header.Value;
                }
            }

            headers["x-retry-count"] = retryCount + 1;

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Headers = headers
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: destination,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            await channel.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            _logger.LogWarning(
                "Prescription message routed to {Queue}, retry count {Count}",
                destination,
                retryCount + 1);
        }

        private static int GetRetryCount(
            IDictionary<string, object?>? headers)
        {
            if (headers == null ||
                !headers.TryGetValue("x-retry-count", out var value) ||
                value == null)
            {
                return 0;
            }

            return value switch
            {
                int count => count,
                long count when count >= 0 && count <= int.MaxValue
                    => (int)count,
                byte[] bytes when int.TryParse(
                    Encoding.UTF8.GetString(bytes),
                    out int count) => count,
                _ => 0
            };
        }
    }
}
