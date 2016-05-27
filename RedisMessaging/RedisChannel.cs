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
using System.Linq;

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

    private IQueue ProcessingQueue { get; set; }

    public IQueue PoisonQueue { get; private set; }

    public IContainer Container { get; private set; }

    public IMessageConverter MessageConverter { get; private set; }

    protected IConnectionMultiplexer _redis;

    private Dictionary<IListener, Queue<IListener>> rr = new Dictionary<IListener, Queue<IListener>>();

    public void Subscribe()
    {
      if (IsSubscribed)
        return;

      var redisConnection = (RedisConnection) Container.Connection;
      _redis = redisConnection.Multiplexer;

      //needs to be more unique per instance
      ProcessingQueue = new RedisQueue(MessageQueue.Name+":Processing", 0);

      for (int i = 0; i<_redis.GetDatabase().ListLength(ProcessingQueue.Name); i++)
      {
        var job = _redis.GetDatabase().ListGetByIndex(ProcessingQueue.Name, i);
        if (!job.IsNullOrEmpty)
          HandleMessage(job);
      }

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
          listenerQueue.Enqueue((RedisListener)listener.Clone());
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
        string key = "";
        var messageObject = MessageConverter.Convert(value, out key);

        //get the type of the key
        var listenerType = (from l in Listeners where l.TypeKey.Equals(key, StringComparison.InvariantCultureIgnoreCase) select l).FirstOrDefault();

        if (listenerType == null)
        {
          //default functionality is to send to poison queue listener is not found
          if (PoisonQueue != null)
          {
            SendToPoisonQueue(value);
            RemoveFromProcessingQueue(value);
          }
          //else, send to HandleException to see how to handle
          else
          {
            throw new Exception("Listener for message not found");
            //HandleException(new Exception("Listener for message not found"), value);
          }
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
          //any exceptions when calling this will go back to HandleException
          //because this is an async void, this thread will keep moving and ignore any errors
          listener.InternalHandlerAsync(messageObject);
        }
      }
      catch(Exception)
      {
        SendToDeadLetterQueue(value);
        RemoveFromProcessingQueue(value);
      }
   
    }

    public void HandleException(Exception e, object m)
    {
      var advice = (from adv in ErrorAdvice where adv.GetType().Equals(e.GetType()) select adv).FirstOrDefault();
      //if nothing in advice chain matches use default error handler
      if (advice == null)
        DefaultErrorHandler.HandleError(e, m);

      if(advice.RetryOnFail)
      {
        //need to determine type of retry
        var retryAdvice = advice as ITimedRetryAdvice;
        if(retryAdvice != null)
        {
          //after programmed delay, send directly to handle message
          Task.Delay(retryAdvice.RetryInterval);
          HandleMessage((RedisValue)m);

          return;
        }
        var retryRequeueAdvice = advice as IRetryRequeueAdvice;
        if(retryRequeueAdvice!=null)
        {
          //remove from processing queue and reque
          
          SendToMessageQueue((RedisValue)m);
          return;
        }
      }
      //if its not retry on fail then...idk, send to default error handler? i guess?
      //en whats the point of advice that doesn't have a retry?
      DefaultErrorHandler.HandleError(e, m);

    }

    internal void SendToPoisonQueue(RedisValue value)
    {
      _redis.GetDatabase().ListLeftPush(PoisonQueue.Name, value);
    }

    internal void SendToMessageQueue(RedisValue value)
    {
      _redis.GetDatabase().ListLeftPush(MessageQueue.Name, value);
    }

    internal void SendToDeadLetterQueue(RedisValue value)
    {
      _redis.GetDatabase().ListLeftPush(DeadLetterQueue.Name, value);
    }

    internal void RemoveFromProcessingQueue(RedisValue value)
    {
      _redis.GetDatabase().ListRemove(ProcessingQueue.Name, value);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      //if (_redis.IsConnected)
      //  _redis.Dispose();
    }
  }
}
