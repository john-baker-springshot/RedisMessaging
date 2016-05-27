using MessageQueue.Contracts.Consumer;
using System;
using System.Linq.Expressions;
using System.Reflection;
using MessageQueue.Contracts;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisMessaging.Consumer
{
  public class RedisListener : IListener
  {
    public IChannel Channel { get; private set; }

    public int Count { get; private set; }

    public string TypeKey { get; private set; }

    public object HandlerType { get; private set; }
    
    public string HandlerMethod { get; private set; }
    

    public async void InternalHandlerAsync(object m)
    {
      var redisChannel = (RedisChannel)Channel;
      try
      {
        //send item to message handler

        //create instance of handler class
        ConstructorInfo constructor = HandlerType.GetType().GetConstructor(Type.EmptyTypes);
        object handlerClass = constructor.Invoke(new object[] { });

        if(handlerClass==null)
          throw new Exception("HandlerType class not found");


        MethodInfo handlerMethod = HandlerType.GetType().GetMethod(HandlerMethod);
        await Task.Run(() =>handlerMethod.Invoke(handlerClass, new object[] { m }));

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

    //public RedisListener CreateInstance()
    //{
    //  return new RedisListener
    //  {
    //    Channel = Channel,
    //    Count = Count,
    //    HandlerType = HandlerType,
    //    HandlerMethod = HandlerMethod,
    //    TypeKey = TypeKey
    //  };
    //}

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
