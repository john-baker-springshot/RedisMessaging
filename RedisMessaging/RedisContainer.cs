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

    //public bool EnableSentinel { get; private set; }

    public RedisQueueSentinel Sentinel { get; private set; }

    public bool EnableSentinel { get; private set; }

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisContainer));

    //private RedisQueueSentinel _sentinel;

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
        RedisQueueSentinel.Instance.Start();
      }
      Log.Info("Redis Container Initialized");
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
