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
using RedisMessaging.Errors;
using RedisMessaging.Producer;

namespace RedisMessaging
{
  public class RedisChannel : IChannel
  {
    public IQueue DeadLetterQueue { get; private set; }

    public IErrorHandler DefaultErrorHandler { get; private set; }

    public IEnumerable<IAdvice> ErrorAdvice { get; private set; }

    public string Id { get; private set; }

    public bool IsSubscribed { get; private set; }

    public IEnumerable<IListener> Listeners { get; private set; }

    public IQueue MessageQueue { get; private set; }

    public IQueue ProcessingQueue { get; private set; }

    public IQueue PoisonQueue { get; private set; }

    public IContainer Container { get; private set; }

    public IMessageConverter MessageConverter { get; private set; }

    public int Count { get; private set; }

    private IConnectionMultiplexer _redis;

    private readonly Dictionary<object, int> _errorDictionary = new Dictionary<object, int>();

    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisChannel));

    private RedisProducer _publisher;

    public void Init()
    {
      var redisConnection = (RedisConnection)Container.Connection;
      _redis = redisConnection.Multiplexer;

      _publisher = new RedisProducer(redisConnection, MessageQueue);

      //need to be able to add in the instance id here...
      if (Id == null)
        Id = "NA";
      var processingQueueName = MessageQueue.Name;
      processingQueueName += ":" + Id + "_" + Environment.MachineName + "_"+DateTime.Now.ToString("yyyyMMdd_HHmmss");

      ProcessingQueue = new RedisQueue(processingQueueName, 0);
    }

    public void Subscribe()
    {
      if (IsSubscribed)
        return;

      Init();

      //TODO: Retest this
      while (_redis.GetDatabase().ListLength(ProcessingQueue.Name)>0)
      {
        var job = _redis.GetDatabase().ListRange(ProcessingQueue.Name, 0, 0).FirstOrDefault();
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
      //.1 seconds
      int interval = 100;
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
        Log.Debug("Handling Message "+value);
        string key = "";
        var messageObject = MessageConverter.Convert(value, out key);

        //get the type of the key
        var listenerType = (from l in Listeners where l.TypeKey.Equals(key, StringComparison.InvariantCultureIgnoreCase) select l).FirstOrDefault() as RedisListener;
        
        if (listenerType == null)
        {
          //default functionality is to send to poison queue listener is not found
          if (PoisonQueue != null)
          {
            Log.Warn("Listener type not found for message "+value);
            SendToPoisonQueue(value);
          }
          else
          {
            throw new Exception("Listener for message not found");
          }
          return;
        }
        Log.Debug("Listener found for message "+value+" of type "+listenerType.TypeKey);
        await listenerType.InternalHandlerAsync(messageObject);
      }
      catch (Exception e)
      {
        Log.Error("Error handling message "+value.ToString(), e);
        HandleException(e, value);
      }
      finally
      {
        RemoveFromProcessingQueue(value);
      }
    }

    internal async void HandleException(Exception e, object m)
    {
      ErrorAdvice advice = null;
      try
      {
        foreach (ErrorAdvice adv in ErrorAdvice)
        {
          if (adv.GetExceptionType() == e.GetType())
            advice = adv;
        }
        //var advice = (from adv in ErrorAdvice where adv.GetExceptionType() == e.GetType() select adv).FirstOrDefault();
        //if nothing in advice chain matches use default error handler
        if (advice == null)
        {
          DefaultErrorHandler.HandleError(e, m);
          return;
        }

        if (advice.RetryOnFail)
        {
          //need to determine type of retry
          //var retryAdvice = advice as ITimedRetryAdvice;
          if (advice.GetAdviceType() == typeof(ITimedRetryAdvice))
          {
            var errorCount = 0;
            _errorDictionary.TryGetValue(m, out errorCount);
            if (errorCount == 0)
              _errorDictionary.Add(m, 0);
            else if (errorCount >= advice.RetryCount)
            {
              SendToDeadLetterQueue(m.ToString());
              return;
            }
            Log.Warn("TimedRetryAdvice found for message "+m+", retrying Handle Message");
            _errorDictionary[m] = errorCount + 1;
            await Task.Delay((advice.RetryInterval*1000));
            HandleMessage(m.ToString());

            return;
          }
          //var retryRequeueAdvice = advice as IRetryRequeueAdvice;
          if (advice.GetAdviceType() == typeof(IRetryRequeueAdvice))
          {
            Log.Warn("RetryRequeue Advice found for message " + m + ", requeing message");
            SendToMessageQueue(m.ToString());
            return;
          }
        }
        //just in case measure
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
      if (_publisher == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _publisher = new RedisProducer(redisConnection);
      }
      Log.Warn("Sending message " + value + " to poison letter queue " + PoisonQueue.Name);
      _publisher.Publish(PoisonQueue.Name, value);
    }

    internal void SendToMessageQueue(RedisValue value)
    {
      if (_publisher == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _publisher = new RedisProducer(redisConnection);
      }
      _publisher.PublishToFront(MessageQueue.Name, value);
    }

    internal void SendToDeadLetterQueue(RedisValue value)
    {
      if (_publisher == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _publisher = new RedisProducer(redisConnection);
      }
      Log.Warn("Sending message " + value + " to dead letter queue " + DeadLetterQueue.Name);
      _publisher.Publish(DeadLetterQueue.Name, value);
    }

    internal void RemoveFromProcessingQueue(RedisValue value)
    {
      if (_redis == null)
      {
        var redisConnection = (RedisConnection)Container.Connection;
        _redis = redisConnection.Multiplexer;
      }
      _redis.GetDatabase().ListRemoveAsync(ProcessingQueue.Name, value);
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

    public object Clone(int instance)
    {
      RedisChannel channel = (RedisChannel)this.MemberwiseClone();
      channel.Id = channel.Id + instance;
      return channel;
    }

    public RedisValue[] GetProcessingMessages()
    {
      return _redis.GetDatabase().ListRange(ProcessingQueue.Name, 0, -1);
    }

  }

}
