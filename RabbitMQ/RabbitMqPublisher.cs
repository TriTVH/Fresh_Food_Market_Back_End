using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMqPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "admin",
                Password = "admin123",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.CallbackException += (_, e) =>
            Console.WriteLine("[Publisher] Channel exception: " + e.Exception.Message);
            _connection.ConnectionShutdown += (_, e) =>
                Console.WriteLine("[Publisher] Connection shutdown: " + e.ReplyText);
        }
        public void Publish(string exchange, string routingKey, byte[] message)
        {
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true
            );

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: message
            );
        }
    }
}
