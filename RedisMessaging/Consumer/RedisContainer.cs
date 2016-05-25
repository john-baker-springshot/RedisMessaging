using MessageQueue.Contracts.ConnectionBase;
using MessageQueue.Contracts.Consumer;
using RedisMessaging.ConnectionBase;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Consumer
{
  public class RedisContainer : IContainer
  {
    public IList<IChannel> Channels { get; set; }

    public IConnection Connection { get; set; }

    public RedisContainer(IConnection connection)
    {
      Connection = connection;
      connection.Connect();

      Channels = new List<IChannel>();
    }

    public void AddChannel(params IChannel[] channels)
    {
      foreach (IChannel channel in channels)
        Channels.Add(channel);
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
