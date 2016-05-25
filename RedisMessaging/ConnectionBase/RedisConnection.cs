using MessageQueue.Contracts.ConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.ConnectionBase
{
  public class RedisConnection : IConnection
  {

    //need a way to turn this on/off
    public bool IsConnected { get; set; }

    public IList<IServer> Servers { get; set; }

    public StackExchange.Redis.IConnectionMultiplexer _redis { get; private set; }

    public RedisConnection()
    {
      Servers = new List<IServer>();
    }

    public void AddServer(IServer server)
    {
      if (server != null)
        Servers.Add(server);
    }

    public void Connect()
    {
      if (IsConnected)
        return;

      //take the connection
      var connStrings = "";
      //itterate through servers
      for (int i = 0; i < Servers.Count; i++)
      {
        if (i == Servers.Count - 1)
        {
          connStrings += Servers[i].Endpoint;
          continue;
        }
        connStrings += Servers[i].Endpoint + ",";
      }
      _redis = StackExchange.Redis.ConnectionMultiplexer.Connect(connStrings);
      if (_redis.IsConnected)
      {
        IsConnected = true;
      }
      else
      {
        throw new Exception("multiplexer cannot connect to endpoint(s)");
      }
    }

    public object GetConnection()
    {
      return _redis;
    }

  }
}
