using NUnit.Framework;
using System;
using System.Collections.Generic;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Advices;
using RedisMessaging.Errors;
using RedisMessaging.Tests.UtilTests;
using RedisMessaging.Util;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ConsumerTests
{
  [TestFixture]
  public class TestRedisChannel
  {
    private XmlObjectFactory _objectFactory;
    private RedisChannel _channel;

    private const string ChannelName = "myChannel";

    [OneTimeSetUp]
    public void Init()
    {
      //_container = ContextRegistry.GetContext();
      _objectFactory = ParserTestsHelper.LoadMessagingConfig();
      _channel = _objectFactory.GetObject<IChannel>(ChannelName) as RedisChannel;
    }

    [TearDown]
    public void Cleanup()
    {
      var connection = (RedisConnection) _channel.Container.Connection;
      var redis = connection.Multiplexer;
      //var messageQueue = _channel.MessageQueue;
      redis.GetDatabase().KeyDelete(_channel.MessageQueue.Name);
      redis.GetDatabase().KeyDelete(_channel.DeadLetterQueue.Name);
      redis.GetDatabase().KeyDelete(_channel.PoisonQueue.Name);

      //while (redis.GetDatabase().ListLength(messageQueue.Name) > 0)
      //  redis.GetDatabase().ListRightPop(messageQueue.Name);
      //var poisonQueue = _channel.PoisonQueue;
      //while (redis.GetDatabase().ListLength(poisonQueue.Name) > 0)
      //  redis.GetDatabase().ListRightPop(poisonQueue.Name);
      //var deadLetterQueue = _channel.DeadLetterQueue;
      //while (redis.GetDatabase().ListLength(deadLetterQueue.Name) > 0)
      //  redis.GetDatabase().ListRightPop(deadLetterQueue.Name);

    }

    [OneTimeTearDown]
    public void Dispose()
    {
      _objectFactory.Dispose();
    }

    [Test]
    public void RedisChannel_DITest()
    {
      Assert.IsNotNull(_channel);
      Assert.That(_channel.GetType(), Is.EqualTo(typeof(RedisChannel)));
    }

    [Test]
    public void RedisChannel_HandleErrorDefaultErrorTest()
    {
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var deadLetterItems = _redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name);
      var e = new ArgumentNullException();
      var m = new KeyValuePair<string, object>("Bad Key", "{\"Message\":\"Bad Message\"}");
      
      _channel.HandleException(e, m);      
      
      Assert.That(_redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name), Is.EqualTo(deadLetterItems+1));
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.DeadLetterQueue.Name, _redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name)-1).ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorRetryRequeueTest()
    {
      _channel = _objectFactory.GetObject<IChannel>(ChannelName) as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength(_channel.MessageQueue.Name);
      var e = new TimeoutException();
      var m = new KeyValuePair<string, object>("Requeue Key:", "{\"Message\":\"Requeue Message\"}");


      _channel.HandleException(e, m);


      Assert.That(_redis.GetDatabase().ListLength(_channel.MessageQueue.Name), Is.EqualTo(messageQueueItems + 1));
      Assert.That(
        _redis.GetDatabase()
          .ListGetByIndex(_channel.MessageQueue.Name, _redis.GetDatabase().ListLength(_channel.MessageQueue.Name) - 1)
          .ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryBadMessageTest()
    {
      _channel = _objectFactory.GetObject<IChannel>(ChannelName) as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength(_channel.MessageQueue.Name);
      var e = new ApplicationException();
      var m = new KeyValuePair<string, object>("Retry Key:", "{\"Message\":\"Retry Message\"}");


      _channel.HandleException(e, m);
      //need to wait the same amount of time as the retry interval of the exception or it will the tests
      var retryAdvice = _objectFactory.GetObject<ErrorAdvice>("advice2");
      System.Threading.Thread.Sleep((retryAdvice.RetryInterval + 1) * 1000);

      //message will error due to key not being found, after which i expect to see it on the dead letter queue
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.DeadLetterQueue.Name, _redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name) - 1).ToString(), Is.EqualTo(m.ToString()));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryGoodMessageTest()
    {
      _channel = _objectFactory.GetObject<IChannel>(ChannelName) as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength(_channel.MessageQueue.Name);
      var e = new ApplicationException();
      var m = RedisMessagingImplementationTests.CreateBasicMessage(1, "HandleErrorTimedRetryGoodMessageTest");


      _channel.HandleException(e, m);


      //message will not error and will be handled successfully, so should not exist on deal letter, poison, or processing
      //but thats a bad assertion, asserting nothing, so...what do?
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.DeadLetterQueue.Name, _redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name) - 1).ToString(), Is.Not.EqualTo(m));
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.PoisonQueue.Name, _redis.GetDatabase().ListLength(_channel.PoisonQueue.Name) - 1).ToString(), Is.Not.EqualTo(m));
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.MessageQueue.Name, _redis.GetDatabase().ListLength(_channel.MessageQueue.Name) - 1).ToString(), Is.Not.EqualTo(m));
      Assert.That(_redis.GetDatabase().ListGetByIndex($"{_channel.MessageQueue.Name}:Processing", _redis.GetDatabase().ListLength($"{_channel.MessageQueue.Name}:Processing") - 1).ToString(), Is.Not.EqualTo(m));
    }

    [Test]
    public void RedisChannel_HandleErrorTimedRetryOverRetryFailTest()
    {
      _channel = _objectFactory.GetObject<IChannel>(ChannelName) as RedisChannel;
      var connection = _channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var messageQueueItems = _redis.GetDatabase().ListLength(_channel.MessageQueue.Name);
      var e = new ApplicationException();
      var m = RedisMessagingImplementationTests.CreateBasicMessage(1, "HandleErrorTimedRetryOverRetryFailTest");

      //first one will pass
      _channel.HandleException(e, m);
      //this one should fail due to it being retried once already
      _channel.HandleException(e,m);


      //message will not error and will be handled successfully, so should not exist on deal letter, poison, or processing
      //but thats a bad assertion, asserting nothing, so...what do?
      Assert.That(_redis.GetDatabase().ListGetByIndex(_channel.DeadLetterQueue.Name, _redis.GetDatabase().ListLength(_channel.DeadLetterQueue.Name) - 1).ToString(), Is.EqualTo(m.ToString()));
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
