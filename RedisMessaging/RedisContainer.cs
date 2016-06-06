using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using MessageQueue.Contracts;

namespace RedisMessaging
{
  public class RedisContainer : IContainer
  {
    public IEnumerable<IChannel> Channels { get; private set; }

    public IConnection Connection { get; private set; }

    public RedisContainer(IConnection connection)
    {
      Connection = connection;
    }

    public bool EnableSentinel { get; private set; }

    public int MessageTimeout { get; private set; }

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisContainer));

    private RedisQueueSentinel _sentinel;

    //need this to init() all channels under it
    public void Init()
    {
      if (!Connection.IsConnected)
        Connection.Connect();
      //take channel list
      //itterate through channels
      foreach (RedisChannel channel in Channels)
      {
        int count = 1;
        Int32.TryParse(channel.Count.ToString() , out count);
        for (int i = 2; i <= channel.Count; i++)
        {
          RedisChannel c = (RedisChannel)channel.Clone(i);
          Channels = Channels.Concat(new [] {c});
        }
      }
      foreach (RedisChannel channel in Channels)
      {
        channel.Subscribe();
      }

      //initialize and start RedisQueueSentinel
      if (EnableSentinel)
      {
        //if no message timeout value set, default to 5 min
        if (MessageTimeout == 0)
          MessageTimeout = 300;//5 minutes
        _sentinel = new RedisQueueSentinel(MessageTimeout);
        new Task(() => RunQueueSentinel(), new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning).Start();
      }
      Log.Info("Redis Container Initialized");
    }

    private void RunQueueSentinel()
    {
      //reach out to container
      //for each Channel
      //sleep for a moment, 10 seconds
      int interval = 10000;
      System.Threading.Thread.Sleep(interval);
      foreach (RedisChannel channel in Channels)
      {
        string queueName = channel.ProcessingQueue.Name;
        //grab all items in a processing queue
        var processingMessages = channel.GetProcessingMessages().ToList();
        _sentinel.Add(queueName, processingMessages);
        _sentinel.Evict(queueName, processingMessages);
        _sentinel.Requeue(channel);
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      foreach (IChannel channel in Channels)
      {
        channel.Dispose();
      }
    }
  }
}
