using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using StackExchange.Redis;

namespace RedisMessaging
{
  public class RedisQueueSentinel: IDisposable
  {
    private readonly object _lock = new object();

    private const int DefaultInterval = 10;

    private const int DefaultMessageTimeout = 300;

    public readonly Dictionary<KeyValuePair<string, RedisValue>, DateTime>  ProcessingMessages = new Dictionary<KeyValuePair<string, RedisValue>, DateTime>();
    public int MessageTimeout { get; private set; }
    public int Interval { get; private set; }
    public bool IsStarted { get; private set; }
    public RedisConnection Connection { get; private set; }
    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisQueueSentinel));
    private readonly IConnectionMultiplexer _redis;

    private RedisQueueSentinel(RedisConnection connection, int processingMessageTimeout, int sentinelInterval)
    {
      Connection = connection;
      
      MessageTimeout = processingMessageTimeout > 0 ? processingMessageTimeout : DefaultMessageTimeout;

      Interval = sentinelInterval > 0 ? sentinelInterval : DefaultInterval;

      _redis = Connection.Multiplexer;

      Log.Info("Sentienl Initialized with timeout " + MessageTimeout);
    }

    private RedisQueueSentinel()
    {
      //declare and leave constructor as private to ensure singleton status
    }

    public void Start()
    {
      lock (_lock)
      {
        if (IsStarted)
          return;

        IsStarted = true;
        new Task(Processing, new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning).Start();
      }
    }

    private void Processing()
    {    
      while (IsStarted)
      {
        var database = Connection.Config.DefaultDatabase.GetValueOrDefault();

        foreach (var endpoint in _redis.GetEndPoints())
        {
          var server = _redis.GetServer(endpoint);
          //$"{MessageQueue.Name}:ProcessingQueue:{Id}_{Environment.MachineName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
          foreach (var key in server.Keys(pattern: "*:ProcessingQueue:*", pageSize: 1000, database: database))
          {
            var processingMessages = _redis.GetDatabase().ListRange(key, 0, -1).ToList();
            Add(key, processingMessages);
            Evict(key, processingMessages);
          }
        }
        Requeue();
        //sleep for a moment
        System.Threading.Thread.Sleep(Interval);
      }
    }

    internal void Add(string queueName, List<RedisValue> messages)
    {
      //add any messages that do not exist
      foreach (var message in messages)
      {
        var kvpKey = new KeyValuePair<string, RedisValue>(queueName, message);
        if (!ProcessingMessages.ContainsKey(kvpKey))
          ProcessingMessages.Add(kvpKey, DateTime.Now);
      }
    }

    internal void Evict(string queueName, List<RedisValue> messages)
    {
      //evict any messages that aren't present anymore
      var itemsToRemove = ProcessingMessages.Where(m => m.Key.Key.Equals(queueName) && !messages.Contains(m.Key.Value)).ToArray();
      foreach (var item in itemsToRemove)
        ProcessingMessages.Remove(item.Key);
    }

    internal void Requeue()
    {
      TimeSpan timeout = TimeSpan.FromSeconds(MessageTimeout);
      var itemsToRequeue = ProcessingMessages.Where(m => m.Value.CompareTo(DateTime.Now-timeout)<1).ToArray();
      foreach (var item in itemsToRequeue)
      {
        var message = item.Key.Value;
        //MessageQueue:id_machineName_datetime
        var messageQueue = item.Key.Key.Split(':')[0];
        //remove from internal dictionary
        ProcessingMessages.Remove(item.Key);
        //remove from processing queue
        _redis.GetDatabase().ListRemove(item.Key.Key, message);
        //push to message queue
        _redis.GetDatabase().ListRightPush(messageQueue, message);
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      IsStarted = false;
    }
  }
}
