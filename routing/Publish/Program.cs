using System;
using System.Text;
using System.Linq;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchange = "direct_logs";
            var severity = (args.Length > 0) ? args[0] : "info";
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "direct");

                string message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: severity,
                    basicProperties: properties,
                    body: body
                );
                
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 1
                ? string.Join(" ", args.Skip(1).ToArray())
                : "Hello World!";
        }
    }
}
