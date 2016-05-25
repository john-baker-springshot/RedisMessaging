using MessageQueue.Contracts.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Consumer
{
  public class RedisMessageHandler : IMessageHandler
  {
    public void HandleMessage(object m)
    {
      Console.WriteLine(m.ToString());
    }
  }
}
