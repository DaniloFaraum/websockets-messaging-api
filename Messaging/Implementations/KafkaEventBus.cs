using Confluent.Kafka;
using WebKafka.Messaging.Interfaces;
using static Confluent.Kafka.ConfigPropertyNames;

namespace WebKafka.Messaging.Implementations
{
    public class KafkaEventBus: IEventBus, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly IConsumer<Null, string> _consumer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public KafkaEventBus()
        {
            var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9094" };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9094",
                GroupId = "chat-messages",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
        }

        public async Task PublishAsync(string topic, string message)
        {
            try { await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message }); }
            catch (ProduceException<Null, string> ex) { Console.WriteLine($"Could not send message to Kafka: {ex.Error.Reason}"); }
        }

        public void Subscribe(string topic, Action<string> handler)
        {
            _consumer.Subscribe(topic);

            Task.Run(() =>
            {
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        var result = _consumer.Consume(_cancellationTokenSource.Token);
                        handler(result.Message.Value);
                    }
                }
            });
        }
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _producer?.Dispose();
            _consumer?.Dispose();
        }
    }
}


    