using MessageQueue.Contracts.Consumer;
using System;
using MessageQueue.Contracts;
using System.Threading.Tasks;

namespace RedisMessaging.Consumer
{
  public class RedisListener : IListener
  {
    public IChannel Channel { get; private set; }

    public int Count { get; private set; }

    public IMessageHandler MessageHandler { get; private set; }

    public Type TypeKey { get; private set; }

    public async void InternalHandlerAsync(object m)
    {
      //send item to message handler
      await new Task(() =>MessageHandler.HandleMessage(m));
      //check for completion
      //if complete, remove from processing queue
      //else throw the message up the chain
    }

    public RedisListener CreateInstance()
    {
      return new RedisListener
      {
        Channel = Channel,
        Count = Count,
        MessageHandler = MessageHandler,
        TypeKey = TypeKey
      };
    }

  }
}
