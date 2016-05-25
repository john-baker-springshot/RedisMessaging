using MessageQueue.Contracts.Consumer;
using System;
using MessageQueue.Contracts;

namespace RedisMessaging.Consumer
{
  public class RedisListener : IListener
  {
    public IChannel Channel { get; }

    public int Count { get; set; }

    public IMessageHandler MessageHandler { get; }

    public Type TypeKey { get; }

    public async void InternalHandlerAsync(object m)
    {
      //send item to message handler
      MessageHandler.HandleMessage(m);
      //check for completion
      //if complete, remove from processing queue
      //else throw the message up the chain
    }
  }
}
