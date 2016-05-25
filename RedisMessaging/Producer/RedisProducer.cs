using MessageQueue.Contracts.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts.ConnectionBase;
using StackExchange.Redis;

namespace RedisMessaging.Producer
{
  public class RedisProducer : IProducer
  {
    public IConnection Connection { get; set; }

    public IQueue MessageQueue { get; set; }

    protected IConnectionMultiplexer _redis;

    public RedisProducer(IConnection connection)
    {
      Connection = connection;
    }
    
    public RedisProducer(IConnection connection, IQueue queue)
    {
      Connection = connection;
      MessageQueue = queue;
    }

    public void Init()
    {
      //take the connection
      var connStrings = "";
      //itterate through servers
      for (int i = 0; i < Connection.Servers.Count; i++)
      {
        if (i == Connection.Servers.Count - 1)
        {
          connStrings += Connection.Servers[i].Endpoint;
          continue;
        }
        connStrings += Connection.Servers[i].Endpoint + ",";
      }

      //create the multiplexer connection
      _redis = ConnectionMultiplexer.Connect(connStrings);
    }

    public void Publish(string message)
    {
      if (MessageQueue == null || MessageQueue.Name == null || MessageQueue.Name.Length < 1)
        throw new Exception("MessageQueue not initialized");

      _redis.GetDatabase().ListLeftPush(MessageQueue.Name, message);
    }

    public void Publish(string queue, string message)
    {
      _redis.GetDatabase().ListLeftPush(queue, message);
    }

    public void Publish(IQueue queue, string message)
    {
      if (queue == null || queue.Name == null || queue.Name.Length < 1)
        throw new Exception("Queue parameter not initialized");

      _redis.GetDatabase().ListLeftPush(queue.Name, message);
    }
  }
}
