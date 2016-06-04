using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using MessageQueue.Contracts;
using Spring.Caching;
using StackExchange.Redis;

namespace RedisMessaging
{
  public class RedisQueueSentinel
  {

    private readonly Dictionary<KeyValuePair<string, RedisValue>, DateTime>  _processingMessages = new Dictionary<KeyValuePair<string, RedisValue>, DateTime>();
    public int MessageTimeout { get; private set; }
    public RedisContainer Container { get; private set; }
    private IConnectionMultiplexer _redis;
    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisQueueSentinel));
    public RedisQueueSentinel(RedisContainer container, int timeout)
    {
      Container = container;
      var redisConnection = container.Connection as RedisConnection;
      _redis = redisConnection.Multiplexer;
      MessageTimeout = timeout;
      Log.Info("Sentienl Initialized with timeout "+timeout);
    }

    public void Start()
    {
      //reach out to container
      //for each Channel
      //10 seconds
      int interval = 10000;
      System.Threading.Thread.Sleep(interval);
      foreach (RedisChannel channel in Container.Channels)
      {
        string queueName = channel.ProcessingQueue.Name;
        var processingMessages = _redis.GetDatabase().ListRange(queueName, 0, -1).ToList();
        Add(queueName, processingMessages);
        Evict(queueName, processingMessages);
        Requeue(channel);
      }
      //sleep for a moment

    }

    private void Add(string queueName, List<RedisValue> messages)
    {
      //add any messages that do not exist
      foreach (var message in messages)
      {
        var kvpKey = new KeyValuePair<string, RedisValue>(queueName, message);
        if (!_processingMessages.ContainsKey(kvpKey))
          _processingMessages.Add(kvpKey, DateTime.Now);
      }
    }

    private void Evict(string queueName, List<RedisValue> messages)
    {
      //evict any messages that aren't present anymore
      foreach (var kvpKey in _processingMessages.Keys)
      {
        //if the key of our kvpKey matches this processingQueueName
        if (queueName.Equals(kvpKey.Key))
        {
          //if our list of scanned messages does not contain the value for this kvpKey
          if (!messages.Contains(kvpKey.Value))
          {
            //remove it
            _processingMessages.Remove(kvpKey);
          }

        }
      }
    }

    private void Requeue(RedisChannel channel)
    {
      TimeSpan timeout = TimeSpan.FromSeconds(MessageTimeout);
      foreach (var item in _processingMessages)
      {
        if (item.Key.Key.Equals(channel.ProcessingQueue.Name) && item.Value.CompareTo(DateTime.Now - timeout) < 1)
        {
          var message = item.Key.Value;
          channel.SendToMessageQueue(message);
          channel.RemoveFromProcessingQueue(message);
          _processingMessages.Remove(item.Key);
        }
      }
    }


  }
}
