using MessageQueue.Contracts.Producer;
using NUnit.Framework;
using RedisMessaging.Producer;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RedisMessaging.Tests.ProducerTests
{
  [TestFixture]
  public class TestRedisProducer
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisProducer_DITest()
    {
      var testObject = _container.GetObject<IProducer>("MyProducer");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisProducer)));
      Assert.That(testObject.MessageQueue.GetType(), Is.EqualTo(typeof(RedisQueue)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisProducer_PublishToDefaultQueueTest()
    {
      const string message = "hey hey hey";
      var producer = _container.GetObject<IProducer>("MyProducer");
      producer.Publish(message);
      //Assert.IsTrue(1 == 1);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(producer.MessageQueue.Name);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void RedisProducer_PublishToStringQueueTest()
    {
      const string message = "ho ho ho";
      const string queue = "notARealQueue";
      var producer = _container.GetObject<IProducer>("MyProducer");
      producer.Publish(queue, message);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void RedisProducer_PublishToIQueueTest()
    {
      const string message = "hee hee hee";
      RedisQueue queue = new RedisQueue("againNotARealQueue", 0);
      var producer = _container.GetObject<IProducer>("MyProducer");
      producer.Publish(queue, message);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue.Name);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void RedisProducer_PublishToDefaultQueueWithKeyTest()
    {
      const string message = "hey hey hey";
      const string key = "Basic";
      var producer = _container.GetObject<IProducer>("MyKeyProducer") as RedisProducer;
      producer.PublishWithKey(message, key);
      //Assert.IsTrue(1 == 1);
      var connection = (RedisConnection)producer.Connection;
      var poppedMessage = connection.Multiplexer.GetDatabase().ListLeftPop(producer.MessageQueue.Name);
      var actualKvp = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(poppedMessage);
      var actualKey = actualKvp.Key;
      var actualMessage = actualKvp.Value;
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
      Assert.IsTrue(actualKey.ToString().Contains(key + ":"));
    }

    [Test]
    public void RedisProducer_PublishToStringQueueWithKeyTest()
    {
      const string message = "ho ho ho";
      const string key = "Basic";
      const string queue = "notARealQueue";
      var producer = _container.GetObject<IProducer>("MyKeyProducer") as RedisProducer;
      producer.PublishWithKey(queue, message, key);
      var connection = (RedisConnection)producer.Connection;
      var poppedMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue);
      var actualKvp = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(poppedMessage);
      var actualKey = actualKvp.Key;
      var actualMessage = actualKvp.Value;
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
      Assert.IsTrue(actualKey.ToString().Contains(key + ":"));
    }

    [Test]
    public void RedisProducer_PublishToIQueueWithKeyTest()
    {
      const string message = "hee hee hee";
      const string key = "Basic";
      RedisQueue queue = new RedisQueue("againNotARealQueue", 0);
      var producer = _container.GetObject<IProducer>("MyKeyProducer") as RedisProducer;
      producer.PublishWithKey(queue, message, key);
      var connection = (RedisConnection)producer.Connection;
      var poppedMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue.Name);
      var actualKvp = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(poppedMessage);
      var actualKey = actualKvp.Key;
      var actualMessage = actualKvp.Value;
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
      Assert.IsTrue(actualKey.ToString().Contains(key + ":"));
    }

  }
}
