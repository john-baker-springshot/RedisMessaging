using MessageQueue.Contracts.Producer;
using System;
using MessageQueue.Contracts;
using StackExchange.Redis;

namespace RedisMessaging.Producer
{
  public class RedisProducer : IProducer
  {
    public IConnection Connection { get; }

    public IQueue MessageQueue { get; }

    private readonly IConnectionMultiplexer _redis;

    public RedisProducer(IConnection connection) : this(connection, null) { }
    
    public RedisProducer(IConnection connection, IQueue queue)
    {
      Connection = connection;
      MessageQueue = queue;

      Connect();

      var redisConnection = (RedisConnection) Connection;
      _redis = redisConnection.Multiplexer;
    }

    public virtual void Publish(string message)
    {
      Connect();

      if (string.IsNullOrEmpty(MessageQueue?.Name))
        throw new Exception("MessageQueue not initialized");

      _redis.GetDatabase().ListLeftPushAsync(MessageQueue.Name, message);
    }

    public virtual void Publish(string queue, string message)
    {
      Connect();

      _redis.GetDatabase().ListLeftPushAsync(queue, message);
    }

    public virtual void Publish(IQueue queue, string message)
    {
      Connect();

      if (string.IsNullOrEmpty(queue?.Name))
        throw new Exception("Queue parameter not initialized");

      _redis.GetDatabase().ListLeftPushAsync(queue.Name, message);
    }

    private void Connect()
    {
      if (!Connection.IsConnected)
        Connection.Connect();
    }
  }
}
