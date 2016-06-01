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

    private MethodInfo _handlerMethod;

    private object _handlerClass;

    public async Task<bool> InternalHandlerAsync(object m)
    {
      //send item to message handler  
      await Task.Run(() => _handlerMethod.Invoke(_handlerClass, new [] { m }));
      return true;
    }

    public void RegisterListener()
    {
      ConstructorInfo constructor = HandlerType.GetType().GetConstructor(Type.EmptyTypes);
      //create instance of handler class
      _handlerClass = constructor.Invoke(new object[] { });

      if (_handlerClass == null)
        throw new Exception("HandlerType class not found");

      var t = Channel.MessageConverter.TypeMapper.GetTypeForKey(TypeKey);

      _handlerMethod = HandlerType.GetType().GetMethod(HandlerMethod, new [] {t});
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

      RedisListener l = new RedisListener
      {
        Channel = Channel,
        Count = Count,
        TypeKey = TypeKey,
        HandlerType = HandlerType,
        HandlerMethod = HandlerMethod
      };

      l.RegisterListener();

      return l;
    }
  }
}
