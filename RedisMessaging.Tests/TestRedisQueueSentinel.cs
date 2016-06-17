using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Producer;
using NUnit.Framework;
using RedisMessaging.Tests.ConsumerTests;
using RedisMessaging.Tests.UtilTests;
using Spring.Context;
using Spring.Context.Support;
using Spring.Objects.Factory.Xml;
using StackExchange.Redis;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class TestRedisQueueSentinel
  {


    private XmlObjectFactory _objectFactory;
    private const string QueueName = "testQueueName";
    private const string SentinelName = "sentinel";

    [SetUp]
    public void Init()
    {
      _objectFactory = ParserTestsHelper.LoadMessagingConfig();
    }
    
    [TearDown]
    public void Dispose()
    {
      _objectFactory.Dispose();
    }


    [Test]
    public void RedisQueueSentienl_AddTest()
    {
      var sentinel = _objectFactory.GetObject<RedisQueueSentinel>(SentinelName);
      var list = new List<RedisValue> {"hey"};
      sentinel.Add(QueueName, list);
      Assert.That(sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "hey")));
    }

    [Test]
    public void RedisQueueSentienl_EvictTest()
    {
      var sentinel = _objectFactory.GetObject<RedisQueueSentinel>(SentinelName);
      var addList = new List<RedisValue> {"heyheyhey"};
      sentinel.Add(QueueName, addList);
      Assert.True(sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "heyheyhey")));

      var evictList = new List<RedisValue> {"hohoho"};
      sentinel.Evict(QueueName, evictList);

      Assert.False(sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "heyheyhey")));
    }

    [Test]
    public void RedisQueueSentienl_RequeueTest()
    {
      var container = _objectFactory.GetObject<IContainer>(TestRedisContainer.ContainerName) as RedisContainer;
      var sentinel = container.Sentinel;
      var channel = container.Channels.FirstOrDefault() as RedisChannel;
      channel.Init();
      const string expectedMessage = "hey";
      var connection = channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      var list = new List<RedisValue>
      {
        expectedMessage
      };

      sentinel.Add(channel.ProcessingQueue.Name, list);
      //sleep for the timeout period
      System.Threading.Thread.Sleep((sentinel.MessageTimeout +  1) * 1000);
      sentinel.Requeue();

      var message = _redis.GetDatabase().ListLeftPop(channel.MessageQueue.Name);
      Assert.That(message.ToString(), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void RedisQueueSentinel_StartTest()
    {
      var container = _objectFactory.GetObject<IContainer>(TestRedisContainer.ContainerName) as RedisContainer;
      var sentinel = container.Sentinel;
      var channel = container.Channels.FirstOrDefault() as RedisChannel;
      channel.Init();
      const string expectedMessage = "hey";
      var connection = channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      _redis.GetDatabase().ListRightPush(channel.ProcessingQueue.Name, expectedMessage);
      
      sentinel.Start();
      //sleep for the timeout period
      System.Threading.Thread.Sleep((sentinel.MessageTimeout + 1) * 1000);
      var message = _redis.GetDatabase().ListLeftPop(channel.MessageQueue.Name);
      Assert.That(message.ToString(), Is.EqualTo(expectedMessage));
    }
  }
}
