using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.Consumer;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Advices;
using RedisMessaging.Util;
using Spring.Validation;
using StackExchange.Redis;

namespace RedisMessaging.Tests.ConsumerTests
{
  [TestFixture]
  public class TestRedisChannel
  {

    private IApplicationContext _container;
    private RedisChannel _channel;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [TearDown]
    public void Dispose()
    {
      var connection = _channel.Container.Connection as RedisConnection;
      var redis = connection.Multiplexer;
      var messageQueue = _channel.MessageQueue;
      while (redis.GetDatabase().ListLength(messageQueue.Name) > 0)
        redis.GetDatabase().ListRightPop(messageQueue.Name);
      var poisonQueue = _channel.PoisonQueue;
      while (redis.GetDatabase().ListLength(poisonQueue.Name) > 0)
        redis.GetDatabase().ListRightPop(poisonQueue.Name);
      var deadLetterQueue = _channel.DeadLetterQueue;
      while (redis.GetDatabase().ListLength(deadLetterQueue.Name) > 0)
        redis.GetDatabase().ListRightPop(deadLetterQueue.Name);
      //var processingQueue = _channel.ProcessingQueue;
      //while (redis.GetDatabase().ListLength(processingQueue.Name) > 0)
      //  redis.GetDatabase().ListRightPop(processingQueue.Name);

    }

    [Test]
    public void RedisChannel_DITest()
    {
      _channel = _container.GetObject<IChannel>("MyChannel") as RedisChannel;
      Assert.IsNotNull(_channel);
      Assert.That(_channel.GetType(), Is.EqualTo(typeof(RedisChannel)));
    }



    [Test]
    public void RedisChannel_HandleErrorDefaultErrorTest()
    {
      _channel = ServiceLocator.GetService<IChannel>("MyChannel") as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var deadLetterItems = _redis.GetDatabase().ListLength("DeadLetterQueue");
      var e = new ArgumentNullException();
      var m = new KeyValuePair<string, object>("Bad Key", "{\"Message\":\"Bad Message\"}");


      _channel.HandleException(e, m);      


      Assert.That(_redis.GetDatabase().ListLength("DeadLetterQueue"), Is.EqualTo(deadLetterItems+1));
      Assert.That(_redis.GetDatabase().ListGetByIndex("DeadLetterQueue", _redis.GetDatabase().ListLength("DeadLetterQueue")-1).ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorRetryRequeueTest()
    {
      _channel = ServiceLocator.GetService<IChannel>("MyChannel") as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength("MessageQueue");
      var e = new TimeoutException();
      var m = new KeyValuePair<string, object>("Requeue Key:", "{\"Message\":\"Requeue Message\"}");


      _channel.HandleException(e, m);


      Assert.That(_redis.GetDatabase().ListLength("MessageQueue"), Is.EqualTo(messageQueueItems + 1));
      Assert.That(
        _redis.GetDatabase()
          .ListGetByIndex("MessageQueue", _redis.GetDatabase().ListLength("MessageQueue") - 1)
          .ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryBadMessageTest()
    {
      _channel = ServiceLocator.GetService<IChannel>("MyChannel") as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength("MessageQueue");
      var e = new ApplicationException();
      var m = new KeyValuePair<string, object>("Retry Key:", "{\"Message\":\"Retry Message\"}");


      _channel.HandleException(e, m);
      //need to wait the same amount of time as the retry internval of the exception or it will the tests
      System.Threading.Thread.Sleep(10000);

      //message will error due to key not being found, after which i expect to see it on the dead letter queue
      Assert.That(_redis.GetDatabase().ListGetByIndex("DeadLetterQueue", _redis.GetDatabase().ListLength("DeadLetterQueue") - 1).ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryGoodMessageTest()
    {
      _channel = ServiceLocator.GetService<IChannel>("MyChannel") as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength("MessageQueue");
      var e = new ApplicationException();
      var m = RedisMessagingImplementationTests.CreateBasicMessage(1, "HandleErrorTimedRetryGoodMessageTest");


      _channel.HandleException(e, m);


      //message will not error and will be handled successfully, so should not exist on deal letter, poison, or processing
      //but thats a bad assertion, asserting nothing, so...what do?
      Assert.That(_redis.GetDatabase().ListGetByIndex("DeadLetterQueue", _redis.GetDatabase().ListLength("DeadLetterQueue") - 1).ToString(), Is.Not.EqualTo(m.ToString()));
      Assert.That(_redis.GetDatabase().ListGetByIndex("PoisonQueue", _redis.GetDatabase().ListLength("PoisonQueue") - 1).ToString(), Is.Not.EqualTo(m.ToString()));
      Assert.That(_redis.GetDatabase().ListGetByIndex("MessageQueue", _redis.GetDatabase().ListLength("MessageQueue") - 1).ToString(), Is.Not.EqualTo(m.ToString()));
      Assert.That(_redis.GetDatabase().ListGetByIndex("MessageQueue:Processing", _redis.GetDatabase().ListLength("MessageQueue:Processing") - 1).ToString(), Is.Not.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryOverRetryFailTest()
    {
      _channel = ServiceLocator.GetService<IChannel>("MyChannel") as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength("MessageQueue");
      var e = new ApplicationException();
      var m = RedisMessagingImplementationTests.CreateBasicMessage(1, "HandleErrorTimedRetryOverRetryFailTest");

      //first one will pass
      _channel.HandleException(e, m);
      //this one should fail due to it being retried once already
      _channel.HandleException(e,m);


      //message will not error and will be handled successfully, so should not exist on deal letter, poison, or processing
      //but thats a bad assertion, asserting nothing, so...what do?
      Assert.That(_redis.GetDatabase().ListGetByIndex("DeadLetterQueue", _redis.GetDatabase().ListLength("DeadLetterQueue") - 1).ToString(), Is.EqualTo(m.ToString()));
    }
    
  }

  public class RetryException : IAdvice
  {
    public int RetryCount { get; set; }

    public int RetryInterval { get; set; }

    public bool RetryOnFail { get; set; }

    Type IAdvice.GetType()
    {
      return typeof(ApplicationException);
    }

  }

}
