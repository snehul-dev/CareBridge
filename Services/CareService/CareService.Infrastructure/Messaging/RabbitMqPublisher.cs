using CareBridge.Contracts.Messaging;
using CareService.Application.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;

namespace CareService.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IConnection _connection;

        public RabbitMqPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishAsync(
            PrescriptionPdfRequested message,
            CancellationToken cancellationToken)
        {
            await using var channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: cancellationToken);

            await PrescriptionQueueTopology.DeclareAsync(
                channel,
                cancellationToken);

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: PrescriptionQueueTopology.MainQueue,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}