using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppExchangeProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> staffList = new List<string>();
            staffList.Add("7630");
            staffList.Add("777");
            //foreach (string staffCode in staffList)
            //{
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "developers", type: "fanout", durable:true);

                    var message = "Hello World !!!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "developers",
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            //}
        }
    }
}
