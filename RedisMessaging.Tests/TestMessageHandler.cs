using MessageQueue.Contracts.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Tests
{
  public class TestMessageHandler
  {
    public void HandleMessage(object m)
    {
      //Task.Delay(5000);
      Console.WriteLine(m.ToString());
    }
  }
}
