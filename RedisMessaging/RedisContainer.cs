using System.Collections.Generic;
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

    //need this to init() all channels under it
    public void Init()
    {
      if (!Connection.IsConnected)
        Connection.Connect();
      //take channel list
      //itterate through channels
      foreach (IChannel channel in Channels)
      {
        channel.Subscribe();
      }
    }
  }
}
