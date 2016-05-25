using MessageQueue.Contracts.ConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.ConnectionBase
{
  public class RedisQueue : IQueue
  {
    public string Name { get; set; }

    public int TTL { get; set; }

    public RedisQueue(string name, int ttl)
    {
      Name = name;
      TTL = ttl;
    }
  }
}
