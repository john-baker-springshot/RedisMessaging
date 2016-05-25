using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Advices;
using MessageQueue.Contracts.Consumer;
using Newtonsoft.Json;
using RedisMessaging.Util;
using StackExchange.Redis;

namespace RedisMessaging
{
  public class RedisChannel : IChannel
  {
    public IQueue DeadLetterQueue { get; set; }

    public IErrorHandler DefaultErrorHandler { get; set; }

    public IList<IAdvice<Exception>> ErrorAdvice { get; set; }

    public string Id { get; set; }

    public bool IsSubscribed { get; set; }

    public IList<IListener> Listeners { get; set; }

    public IQueue MessageQueue { get; set; }

    public IQueue ProcessingQueue { get; set; }

    public IQueue PoisonQueue { get; set; }

    public ITypeMapper TypeMapper { get; set; }

    public IContainer Container { get; set; }

    protected IConnectionMultiplexer _redis;

    private Dictionary<IListener, Queue<IListener>> rr = new Dictionary<IListener, Queue<IListener>>();

    public void Connect(RedisConnection conn)
    {

    }

    public void Subscribe()
    {
      if (IsSubscribed)
        return;

      var redisConnection = (RedisConnection) Container.Connection;
      _redis = redisConnection.Multiplexer;

      ConnectListeners();

      //start a new thread so we dont get trapped in it
      new Task(Poll, new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning).Start();

      IsSubscribed = true;
    }

    public void ConnectListeners()
    {
      //set up listener instances
      foreach (IListener listener in Listeners)
      {
        Queue<IListener> listenerQueue = new Queue<IListener>();

        for (int i = 0; i < listener.Count; i++)
        {
          listenerQueue.Enqueue((IListener)Activator.CreateInstance(listener.GetType()));
        }
        rr.Add(listener, listenerQueue);
      }
    }

    private void Poll()
    {
      for(;;)
      {
        //5 seconds
        int interval = 5000;
        System.Threading.Thread.Sleep(interval);
        //polling subscribe pattern
        //continuously poll the queue
        if (Container.Connection.IsConnected)
        {
          do
          {
            string job = _redis.GetDatabase().ListRightPopLeftPush(MessageQueue.Name, ProcessingQueue.Name);
            HandleMessage(job);
          } while (true);

          //send job to typemapper, then to appropriate IListener
        }
        else
        {
          IsSubscribed = false;
          break;
        }
      }
      
    }

    private void HandleMessage(RedisValue value)
    {
      try
      {
        //pull the key out of the message
        var message = JsonConvert.DeserializeObject<KeyValuePair<string, object>>(value);

        //need to figure this out programatically, not hard coded this way...
        var key = TypeMapper.GetTypeForKey(message.Key.Split(':')[0]);

        //get the type of the key
        var listenerType = ServiceLocator.GetService<IListener>(key);

        if(listenerType==null)
        {
          //need to check to make sure poison queue is set up
          _redis.GetDatabase().ListLeftPush(PoisonQueue.Name, value);
          _redis.GetDatabase().ListRemove(ProcessingQueue.Name, value);
          return;
        }

        //get the next key in RR
        Queue<IListener> queue;

        rr.TryGetValue(listenerType, out queue);

        //pop first listener, then push to the pack of the queue
        if (queue != null)
        {
          var listener = queue.Dequeue();
          queue.Enqueue(listener);
          //Call on the internal handler
          //TODO: This should be async
          listener.InternalHandlerAsync(value);
        }
      }
      catch (Exception)
      {
        //need to handle exceptions first through advice chain
        //need to check to make sure dead letter queue is set up
        _redis.GetDatabase().ListLeftPush(DeadLetterQueue.Name, value);
        _redis.GetDatabase().ListRemove(ProcessingQueue.Name, value);
      }
      
    }

    private void PubSub()
    {
      //pub/sub subscribe pattern
    }

    private void BPop()
    {
      //blocking pop subscribe pattern
    }
  }
}
