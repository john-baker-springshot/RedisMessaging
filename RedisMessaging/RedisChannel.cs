using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Advices;
using MessageQueue.Contracts.Consumer;
using Newtonsoft.Json;
using RedisMessaging.Util;
using StackExchange.Redis;
using RedisMessaging.Consumer;

namespace RedisMessaging
{
  public class RedisChannel : IChannel
  {
    public IQueue DeadLetterQueue { get; private set; }

    public IErrorHandler DefaultErrorHandler { get; private set; }

    public IEnumerable<IAdvice<Exception>> ErrorAdvice { get; private set; }

    public string Id { get; }

    public bool IsSubscribed { get; private set; }

    public IEnumerable<IListener> Listeners { get; private set; }

    public IQueue MessageQueue { get; private set; }

    public IQueue ProcessingQueue { get; private set; }

    public IQueue PoisonQueue { get; private set; }

    public ITypeMapper TypeMapper { get; private set; }

    public IContainer Container { get; private set; }

    protected IConnectionMultiplexer _redis;

    private Dictionary<IListener, Queue<IListener>> rr = new Dictionary<IListener, Queue<IListener>>();

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
      if (IsSubscribed)
        return;

      //set up listener instances
      foreach (RedisListener listener in Listeners)
      {
        Queue<IListener> listenerQueue = new Queue<IListener>();

        for (int i = 0; i < listener.Count; i++)
        {
          listenerQueue.Enqueue(listener.CreateInstance());
        }
        rr.Add(listener, listenerQueue);
      }
    }

    private void Poll()
    {
      //5 seconds
      int interval = 5000;
      //polling subscribe pattern
      //continuously poll the queue
      if (Container.Connection.IsConnected)
      {
        do
        {
          var job = _redis.GetDatabase().ListRightPopLeftPush(MessageQueue.Name, ProcessingQueue.Name);
          if(!job.IsNullOrEmpty)
            HandleMessage(job);
          else
            System.Threading.Thread.Sleep(interval);
        } while (true);

        //send job to typemapper, then to appropriate IListener
      }
      else
      {
        IsSubscribed = false;
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
