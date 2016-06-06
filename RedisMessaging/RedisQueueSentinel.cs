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

    public readonly Dictionary<KeyValuePair<string, RedisValue>, DateTime>  ProcessingMessages = new Dictionary<KeyValuePair<string, RedisValue>, DateTime>();
    public int MessageTimeout { get; }
    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisQueueSentinel));

    public RedisQueueSentinel(int timeout)
    {
      MessageTimeout = timeout;
      Log.Info("Sentienl Initialized with timeout "+timeout);
    }

    public void Add(string queueName, List<RedisValue> messages)
    {
      //add any messages that do not exist
      foreach (var message in messages)
      {
        var kvpKey = new KeyValuePair<string, RedisValue>(queueName, message);
        if (!ProcessingMessages.ContainsKey(kvpKey))
          ProcessingMessages.Add(kvpKey, DateTime.Now);
      }
    }

    public void Evict(string queueName, List<RedisValue> messages)
    {
      //evict any messages that aren't present anymore
      var itemsToRemove = ProcessingMessages.Where(m => m.Key.Key.Equals(queueName) && !messages.Contains(m.Key.Value)).ToArray();
      foreach (var item in itemsToRemove)
        ProcessingMessages.Remove(item.Key);
      //foreach (var kvpKey in ProcessingMessages.Keys)
      //{
      //  //if the key of our kvpKey doesnt match this processingQueueName, continue
      //  if (!queueName.Equals(kvpKey.Key))
      //    continue;

      //  //if our list of scanned messages does not contain the value for this kvpKey
      //  if (!messages.Contains(kvpKey.Value))
      //  {
      //    //remove it
      //    ProcessingMessages.Remove(kvpKey);
      //  }
      //}
    }

    public void Requeue(RedisChannel channel)
    {
      TimeSpan timeout = TimeSpan.FromSeconds(MessageTimeout);
      var itemsToRequeue = ProcessingMessages.Where(m => m.Key.Key.Equals(channel.ProcessingQueue.Name) && m.Value.CompareTo(DateTime.Now-timeout)<1).ToArray();
      foreach (var item in itemsToRequeue)
      {
        var message = item.Key.Value;
        channel.SendToMessageQueue(message);
        channel.RemoveFromProcessingQueue(message);
        ProcessingMessages.Remove(item.Key); 
      }
    }


  }
}
