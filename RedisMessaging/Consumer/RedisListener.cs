using MessageQueue.Contracts.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;

namespace RedisMessaging.Consumer
{
  public class RedisListener : IListener
  {
    public IContainer Container { get; set; }

    public int Count { get; set; }

    public IMessageHandler MessageHandler { get; set; }

    public Type TypeKey { get; set; }

    public void InternalHandler(object m)
    {
      //send item to message handler
      MessageHandler.HandleMessage(m);
      //check for completion
      //if complete, remove from processing queue
      //else throw the message up the chain
    }
  }
}
