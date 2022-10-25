using System.Text;
using Azure.Messaging.ServiceBus;
using ConsistencyService.EventProcessing;

namespace ConsistencyService.MessageBus
{
    public class MessageBusClient : BackgroundService
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private static ServiceBusClient client;
        private static ServiceBusProcessor processor;
        private readonly IEventProcessor _eventProcessor;

        public MessageBusClient(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
            _connectionString = configuration.GetConnectionString("ServiceBus");
            _queueName = configuration["QueueName"];
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var message = Encoding.UTF8.GetString(args.Message.Body.ToArray());
            Console.WriteLine($"Received: {message}");

            await _eventProcessor.ProcessEventAsync(Encoding.UTF8.GetString(args.Message.Body.ToArray()));

            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var clientOptions = new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets };
            client = new ServiceBusClient(_connectionString, clientOptions);

            processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync(stoppingToken);

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync(stoppingToken);
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
