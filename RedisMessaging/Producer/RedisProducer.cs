using MessageQueue.Contracts.Producer;
using System;
using System.Collections.Generic;
using Common.Logging;
using MessageQueue.Contracts;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisMessaging.Producer
{
  public class RedisProducer : IProducer
  {
    public IConnection Connection { get; }

    public IQueue MessageQueue { get; }

    public bool CreateKey { get; private set; }

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

    public virtual void PublishWithKey(string message, string key)
    {
      Connect();
      if (string.IsNullOrEmpty(MessageQueue?.Name))
        throw new Exception("MessageQueue not initialized");
      if (CreateKey)
        message = CreateMessageKey(message, key);

      _redis.GetDatabase().ListLeftPushAsync(MessageQueue.Name, message);
    }

    public virtual void PublishWithKey(string queue, string message, string key)
    {
      Connect();
      if (CreateKey)
        message = CreateMessageKey(message, key);
      _redis.GetDatabase().ListLeftPushAsync(queue, message);
    }

    public virtual void PublishWithKey(IQueue queue, string message, string key)
    {
      Connect();

      if (string.IsNullOrEmpty(queue?.Name))
        throw new Exception("Queue parameter not initialized");
      if (CreateKey)
        message = CreateMessageKey(message, key);
      _redis.GetDatabase().ListLeftPushAsync(queue.Name, message);
    }

    private string CreateMessageKey(string message, string key)
    {
      key = key + ":" + System.Guid.NewGuid().ToString();
      KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(key, message);
      return JsonConvert.SerializeObject(newMessage);
    }

    private void Connect()
    {
      if (!Connection.IsConnected)
        Connection.Connect();
    }
  }
}
