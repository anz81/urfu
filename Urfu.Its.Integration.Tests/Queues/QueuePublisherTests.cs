using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.Integration.Queues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Urfu.Its.Integration.Queues
{
    [TestClass]
    public class QueuePublisherTests
    {
        [TestMethod]
        public void PostDataTest()
        {
            var publishData = new List<byte[]>();

            var factory = MockConnectionFactory(publishData);
            var publisher = new QueuePublisher { Exchange = "exchange", Factory = factory };

            var data = new [] { new { id = 34, name = "hello" } };
            publisher.PostData(data, "routingKey", null);

            Assert.IsTrue(publishData.Count == 1, "Данные не отправляются");

            var json = Encoding.UTF8.GetString(publishData.First());
            Assert.AreEqual("{\"id\":34,\"name\":\"hello\"}", json, "Нет преобразования в json");
        }

        private static IConnectionFactory MockConnectionFactory(ICollection<byte[]> published)
        {
            var con = new Mock<IConnection>();
            // Создание канала
            con.Setup(c => c.CreateModel()).Returns(() =>
            {
                var ch = new Mock<IModel>();
                // Создание свойств
                ch.Setup(m => m.CreateBasicProperties()).Returns(new BasicProperties());
                // Публикация данных в список
                ch.Setup(m => m.BasicPublish("exchange", "routingKey", It.IsAny<IBasicProperties>(), It.IsAny<byte[]>()))
                    .Callback((string exchange, string routingKey, object props, byte[] bytes) => published.Add(bytes));
                return ch.Object;
            });

            return Mock.Of<IConnectionFactory>(f => f.CreateConnection() == con.Object);
        }
    }
}