using Azure.Messaging.ServiceBus;
using DAL.Models;
using System.Text.Json;

namespace PersonCommands.MessageBus
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly string _connectionString;
        private readonly string _queueName;

        public MessageBusClient(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ServiceBus");
            _queueName = configuration["QueueName"];

            var clientOptions = new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _client = new ServiceBusClient(_connectionString, clientOptions);
            _sender = _client.CreateSender(_queueName);
        }

        public async Task PublishNewPerson(Person person)
        {
            var message = JsonSerializer.Serialize(person);

            if (!_client.IsClosed)
            {
                try
                {
                    await _sender.SendMessageAsync(new ServiceBusMessage(message));

                    Console.WriteLine("A message has been sent from CommandService.");
                }
                finally
                {
                    await _sender.DisposeAsync();
                    await _client.DisposeAsync();
                }
            }
        }
    }
}
