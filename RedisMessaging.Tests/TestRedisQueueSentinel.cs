using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Producer;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using StackExchange.Redis;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class TestRedisQueueSentinel
  {


    private IApplicationContext _container;
    private const string QueueName = "testQueueName";

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }


    [Test]
    public void RedisQueueSentienl_AddTest()
    {
      var sentinel = new RedisQueueSentinel(10);
      List<RedisValue> list = new List<RedisValue>();
      list.Add("hey");
      sentinel.Add(QueueName, list);
      Assert.That(sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "hey")));
    }

    [Test]
    public void RedisQueueSentienl_EvictTest()
    {
      var sentinel = new RedisQueueSentinel(10);
      List<RedisValue> addList = new List<RedisValue>();
      addList.Add("heyheyhey");
      sentinel.Add(QueueName, addList);
      Assert.That(sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "heyheyhey")));
      List<RedisValue> evictList = new List<RedisValue>();
      evictList.Add("hohoho");
      sentinel.Evict(QueueName, evictList);
      Assert.That(!sentinel.ProcessingMessages.ContainsKey(new KeyValuePair<string, RedisValue>(QueueName, "heyheyhey")));
    }

    [Test]
    public void RedisQueueSentienl_RequeueTest()
    {
      var sentinel = new RedisQueueSentinel(10);
      var channel = _container.GetObject<IChannel>("MyChannel") as RedisChannel;
      channel.Init();
      const string expectedMessage = "hey";
      var connection = channel.Container.Connection as RedisConnection;
      var _redis = connection.Multiplexer;
      List<RedisValue> list = new List<RedisValue>();
      list.Add(expectedMessage);

      sentinel.Add(channel.ProcessingQueue.Name, list);
      //sleep for the timeout period
      System.Threading.Thread.Sleep(10000);
      sentinel.Requeue(channel);

      var message = _redis.GetDatabase().ListLeftPop(channel.MessageQueue.Name);
      Assert.That(message.ToString(), Is.EqualTo(expectedMessage));
    }
  }
}
