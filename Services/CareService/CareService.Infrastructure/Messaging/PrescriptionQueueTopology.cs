using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Messaging
{
    public class PrescriptionQueueTopology
    {
        public const string MainQueue = "prescription.pdf.generate";
        public const string RetryQueue = "prescription.pdf.retry";
        public const string FailedQueue = "prescription.pdf.failed";

        public const int MaxRetries = 3;

        public static async Task DeclareAsync(
            IChannel channel,
            CancellationToken cancellationToken)
        {
            // Failed messages are preserved here
            await channel.QueueDeclareAsync(
                queue: FailedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            // Retry queue waits 10 seconds before
            // routing messages back to main queue
            await channel.QueueDeclareAsync(
                queue: RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    ["x-message-ttl"] = 10000,
                    ["x-dead-letter-exchange"] = "",
                    ["x-dead-letter-routing-key"] = MainQueue
                },
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: MainQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);
        }
    }
}
