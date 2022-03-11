using System;
using System.Text;
using RabbitMQ.Client;

namespace HttpMicroservice.Repositories.Queue
{
    public class QueueRepository : IQueueRepository
    {
        public void Enqueue<T>(T obj, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: jsonBytes);
            }

        }
    }
}

