using MessageQueue.Contracts.ConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.ConnectionBase
{
  public class RedisServer : IServer
  {
    public string Endpoint { get; set; }

    public string Pass { get; set; }

    public string Name { get; set; }
    
    public string User { get; set; }



    public RedisServer(string endpoint)
    {
      Endpoint = endpoint;
    }

  }
}
