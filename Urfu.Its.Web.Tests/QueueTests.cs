using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Queues;

namespace Urfu.Its.Web.Tests
{
    [TestClass]
    public class QueueTests
    {
        [TestMethod]
        public void testVariantsQueuePushPop()
        {
            var sendedVariant = new VariantApiDto()
            {
                direction = new DirectionApiDto() {uid = "тестовый объект"}
            };
            PersonalCabinetService.PostVariant(sendedVariant);
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQServer"]
            };
            string RabbitMQQueueName = ConfigurationManager.AppSettings["RabbitMQVariantsQueueName"];
            string RabbitMQRoutingKey = ConfigurationManager.AppSettings["RabbitMQVariantsRoutingKey"];

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(RabbitMQQueueName, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(RabbitMQRoutingKey, true, consumer);

                        var ea = (BasicDeliverEventArgs) consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                    var recivedVariant = JsonConvert.DeserializeObject<VariantApiDto>(message);
                    Assert.AreEqual(recivedVariant.direction.uid, sendedVariant.direction.uid);
                }
            }
        }
    }
}
