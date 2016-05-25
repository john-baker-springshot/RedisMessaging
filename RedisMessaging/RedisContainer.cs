using System.Collections.Generic;
using MessageQueue.Contracts;

namespace RedisMessaging
{
  public class RedisContainer : IContainer
  {
    public IEnumerable<IChannel> Channels { get; }

    public IConnection Connection { get; }

    public RedisContainer(IConnection connection)
    {
      Connection = connection;
      connection.Connect();

      Channels = new List<IChannel>();
    }

    //need this to init() all channels under it
    public void Init()
    {
      //take channel list
      //itterate through channels
      foreach (IChannel channel in Channels)
      {
        channel.Subscribe();
      }
    }
  }
}
