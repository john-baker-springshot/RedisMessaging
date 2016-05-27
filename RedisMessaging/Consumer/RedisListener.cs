using MessageQueue.Contracts.Consumer;
using System;
using MessageQueue.Contracts;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisMessaging.Consumer
{
  public class RedisListener : IListener
  {
    public IChannel Channel { get; private set; }

    public int Count { get; private set; }

    public IMessageHandler MessageHandler { get; protected internal set; }

    public string TypeKey { get; set; }

    public async void InternalHandlerAsync(object m)
    {
      var redisChannel = (RedisChannel)Channel;
      try
      {
        //send item to message handler
        await Task.Run(()=> MessageHandler.HandleMessage(m));
        //check for completion
        //if complete, remove from processing queue
        
        redisChannel.RemoveFromProcessingQueue((RedisValue)m);
      }
      catch (Exception)
      {
        redisChannel.SendToDeadLetterQueue((RedisValue)m);
        //redisChannelRemoveFromProcessingQueue((RedisValue)m);
      }
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
