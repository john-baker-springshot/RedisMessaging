using MessageQueue.Contracts.Consumer;
using System;
using System.Linq.Expressions;
using System.Reflection;
using MessageQueue.Contracts;
using System.Threading.Tasks;
using Common.Logging;
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

    public bool isRegistered { get; private set; }

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisListener));

    public async Task<bool> InternalHandlerAsync(object m)
    {
      if(!isRegistered)
        RegisterListener();
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
      Log.Info("Listener Type "+TypeKey+" Registered");
    }
  }
}
