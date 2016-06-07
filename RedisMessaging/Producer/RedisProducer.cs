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

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisProducer));

    public RedisProducer(IConnection connection) : this(connection, null) { }
    
    public RedisProducer(IConnection connection, IQueue queue)
    {
      Connection = connection;
      MessageQueue = queue;

      Connect();

      var redisConnection = (RedisConnection) Connection;
      _redis = redisConnection.Multiplexer;
      Log.Info("Producer Initialized");
    }

    public virtual void Publish(string message)
    {
      Connect();
      if (string.IsNullOrEmpty(MessageQueue?.Name))
        throw new Exception("MessageQueue not initialized");
      
      _redis.GetDatabase().ListLeftPushAsync(MessageQueue.Name, message);
      Log.Debug("Sending message "+message+" to "+MessageQueue.Name);
    }

    public virtual void Publish(string queue, string message)
    {
      Connect();

      _redis.GetDatabase().ListLeftPushAsync(queue, message);
      Log.Debug("Sending message " + message + " to " + queue);
    }

    public virtual void Publish(IQueue queue, string message)
    {
      Connect();

      if (string.IsNullOrEmpty(queue?.Name))
        throw new Exception("Queue parameter not initialized");

      _redis.GetDatabase().ListLeftPushAsync(queue.Name, message);
      Log.Debug("Sending message " + message + " to " + queue.Name);
    }

    public virtual void PublishToFront(string queueName, string message)
    {
      _redis.GetDatabase().ListRightPushAsync(queueName,message);
    }

    private void Connect()
    {
      if (!Connection.IsConnected)
        Connection.Connect();
    }
  }
}
