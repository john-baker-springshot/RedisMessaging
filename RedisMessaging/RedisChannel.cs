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
using Common.Logging;

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

    public IQueue ProcessingQueue { get; set; }

    public IQueue PoisonQueue { get; private set; }

    public IContainer Container { get; private set; }

    public IMessageConverter MessageConverter { get; private set; }

    protected IConnectionMultiplexer _redis;

    private readonly Dictionary<object, int> _errorDictionary = new Dictionary<object, int>();

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisChannel));

    public void Subscribe()
    {
      if (IsSubscribed)
        return;

      var redisConnection = (RedisConnection)Container.Connection;
      _redis = redisConnection.Multiplexer;

      //needs to be more unique per instance
      ProcessingQueue = new RedisQueue(MessageQueue.Name+":Processing", 0);
      
      while(_redis.GetDatabase().ListLength(ProcessingQueue.Name)>0)
      {
        var job = _redis.GetDatabase().ListRange(ProcessingQueue.Name, 0, 0).First();
        if (!job.IsNullOrEmpty)
          HandleMessage(job);
      }

      ConnectListeners();

      IsSubscribed = true;

      //start a new thread so we dont get trapped in it
      new Task(Poll, new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning).Start();
      Log.Info("Redis Channel subscribed");
    }

    public void ConnectListeners()
    {
      if (IsSubscribed)
        return;

      //set up listener instances
      foreach (RedisListener listener in Listeners)
      {
        listener.RegisterListener();
      }
    }

    private void Poll()
    {
      //.5 seconds
      int interval = 500;
      //polling subscribe pattern
      //continuously poll the queue
      if (Container.Connection.IsConnected)
      {
        do
        {
          var job = _redis.GetDatabase().ListRightPopLeftPush(MessageQueue.Name, ProcessingQueue.Name);
          if (!job.IsNullOrEmpty)
            HandleMessage(job);
          else
          {
            System.Threading.Thread.Sleep(interval);
          }
            
        } while (IsSubscribed);
      }
      else
      {
        IsSubscribed = false;
      }
    }

    private async void HandleMessage(RedisValue value)
    {
      try
      {
        string key = "";
        var messageObject = MessageConverter.Convert(value, out key);

        //get the type of the key
        var listenerType = (from l in Listeners where l.TypeKey.Equals(key, StringComparison.InvariantCultureIgnoreCase) select l).FirstOrDefault() as RedisListener;

        if (listenerType == null)
        {
          //default functionality is to send to poison queue listener is not found
          if (PoisonQueue != null)
          {
            SendToPoisonQueue(value);
          }
          else
          {
            throw new Exception("Listener for message not found");
          }
          return;
        }

        await listenerType.InternalHandlerAsync(messageObject);
      }
      catch (Exception e)
      {
        Log.Error("Error handling message "+value.ToString()+": "+e.Message);
        HandleException(e, value);
      }
      finally
      {
        RemoveFromProcessingQueue(value);
      }
    }

    internal async void HandleException(Exception e, object m)
    {
      try
      {
        var advice = (from adv in ErrorAdvice where adv.GetType() == e.GetType() select adv).FirstOrDefault();
        //if nothing in advice chain matches use default error handler
        if (advice == null)
        {
          DefaultErrorHandler.HandleError(e, m);
          return;
        }

        if (advice.RetryOnFail)
        {
          //need to determine type of retry
          var retryAdvice = advice as ITimedRetryAdvice<Exception>;
          if (retryAdvice != null)
          {
            var errorCount = 0;
            _errorDictionary.TryGetValue(m, out errorCount);
            if (errorCount == 0)
              _errorDictionary.Add(m, 0);
            else if (errorCount >= retryAdvice.RetryCount)
            {
              SendToDeadLetterQueue(m.ToString());
              return;
            }

            _errorDictionary[m] = errorCount + 1;
            await Task.Delay((retryAdvice.RetryInterval*1000));
            HandleMessage(m.ToString());

            return;
          }
          var retryRequeueAdvice = advice as IRetryRequeueAdvice<Exception>;
          if (retryRequeueAdvice != null)
          {
            SendToMessageQueue(m.ToString());
            return;
          }
        }
        DefaultErrorHandler.HandleError(e, m);
      }
      catch (Exception ex)
      {
        Log.Error("Error handling exception for" + m.ToString() + ": " + ex.Message);
        DefaultErrorHandler.HandleError(ex, m);
      }
    }


    internal void SendToPoisonQueue(RedisValue value)
    {
      if (_redis == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _redis = redisConnection.Multiplexer;
      }
      _redis.GetDatabase().ListLeftPush(PoisonQueue.Name, value);
    }

    internal void SendToMessageQueue(RedisValue value)
    {
      if (_redis == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _redis = redisConnection.Multiplexer;
      }
      _redis.GetDatabase().ListLeftPush(MessageQueue.Name, value);
    }

    internal void SendToDeadLetterQueue(RedisValue value)
    {
      if (_redis == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _redis = redisConnection.Multiplexer;
      }
      _redis.GetDatabase().ListLeftPush(DeadLetterQueue.Name, value);
    }

    internal void RemoveFromProcessingQueue(RedisValue value)
    {
      if (_redis == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _redis = redisConnection.Multiplexer;
      }
      _redis.GetDatabase().ListRemove(ProcessingQueue.Name, value);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_redis!=null && _redis.IsConnected)
        _redis.Dispose();
      IsSubscribed = false;
    }
  }

}
