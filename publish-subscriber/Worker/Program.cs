using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
             var queue = "task_queue";
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                // only fetch one message at a time and wait until it finish processing it
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

               var consumer = new EventingBasicConsumer(channel);
               consumer.Received += (model, ea) =>
               {
                   var body = ea.Body;
                   var message = Encoding.UTF8.GetString(body);
                   Console.WriteLine( "[x] Received: {0}", message);

                   int dots = message.Split('.').Length - 1;
                   Thread.Sleep(dots * 1000);

                   Console.WriteLine(" [x] Done");
                    
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
               };
                
                channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

                Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
            }
        }
    }
}
