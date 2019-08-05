using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var queue = "hello";
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

               var consumer = new EventingBasicConsumer(channel);
               consumer.Received += (model, ea) =>
               {
                   var body = ea.Body;
                   var message = Encoding.UTF8.GetString(body);
                   Console.WriteLine( "[x] Received: {0}", message);
               };
                
                channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

                Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
            }
        }
    }
}
