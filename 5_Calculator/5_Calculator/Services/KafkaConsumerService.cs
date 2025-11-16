using _5_Calculator.Controllers;
using _5_Calculator.Data;
using _5_Calculator.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.Metrics;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace _5_Calculator.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _topic;

        private readonly IConsumer<Null, string> _kafkaConsumer;

        private readonly IServiceProvider _serviceProvider;

        private readonly IHttpClientFactory _clientFactory;

        public KafkaConsumerService(IConfiguration config, IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        {
            var consumerConfig = new ConsumerConfig();

            config.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);

            _topic = config.GetValue<string>("Kafka:TopicName");

            _kafkaConsumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();

            _serviceProvider = serviceProvider;

            _clientFactory = clientFactory;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _kafkaConsumer.Consume(cancellationToken);
                    var ip = cr.Message.Value;
                    var inputData = JsonSerializer.Deserialize<DataInputVariant>(cr.Message.Value);
                    var result = CalculatorLibrary.CalculateOperation(inputData.Operand_1, inputData.Operand_2, inputData.Type_operation);
                    inputData.Result = result.ToString();
                    var httpClient = _clientFactory.CreateClient();
                    await httpClient.PostAsJsonAsync($"http://localhost:5005/5_Calculator/Callback", inputData);
                    Console.WriteLine($"Message key: {cr.Message.Key}, value: {cr.Message.Value}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }
        public override void Dispose()
        {
            _kafkaConsumer.Close(); 
            _kafkaConsumer.Dispose();
            base.Dispose();
        }
    }
}