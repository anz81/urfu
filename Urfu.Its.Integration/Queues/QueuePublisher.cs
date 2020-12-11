using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Urfu.Its.Integration.Queues
{
    public class QueuePublisher
    {
        public string Exchange { get; set;  }
        public IConnectionFactory Factory { get; set; }


        public void PostData(IEnumerable<object> objs, string routingKey, IDictionary<string, object> headers)
        {
            using (var connection = Factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var props = channel.CreateBasicProperties();
                    props.ContentType = "application/json";
                    if (null != headers)
                    {
                        props.Headers = headers;
                    }

                    foreach (var obj in objs)
                    {
                        var json = JsonConvert.SerializeObject(obj);
                        var bytes = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(Exchange, routingKey, props, bytes);
                    }
                }
            }
        }
    }
}