using MessageQueue.Contracts.Producer;
using NUnit.Framework;
using RedisMessaging.Producer;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ProducerTests
{
  [TestFixture]
  public class TestRedisProducer
  {
    private XmlObjectFactory _objectFactory;
    private const string ProducerName = "myProducer";


    [OneTimeSetUp]
    public void Init()
    {
      _objectFactory = ParserTestsHelper.LoadMessagingConfig();
    }

    [OneTimeTearDown]
    public void Dispose()
    {
      _objectFactory.Dispose();
    }

    [Test]
    public void RedisProducer_DITest()
    {
      var testObject = _objectFactory.GetObject<IProducer>(ProducerName);
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisProducer)));
      Assert.That(testObject.Queue.GetType(), Is.EqualTo(typeof(RedisQueue)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisProducer_PublishToDefaultQueueTest()
    {
      const string message = "hey hey hey";
      var producer = _objectFactory.GetObject<IProducer>(ProducerName);
      producer.Publish(message);
      //Assert.IsTrue(1 == 1);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(producer.Queue.Name);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void RedisProducer_PublishToStringQueueTest()
    {
      const string message = "ho ho ho";
      const string queue = "notARealQueue";
      var producer = _objectFactory.GetObject<IProducer>(ProducerName);
      producer.Publish(queue, message);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void RedisProducer_PublishToIQueueTest()
    {
      const string message = "hee hee hee";
      var queue = new RedisQueue("againNotARealQueue", 0);
      var producer = _objectFactory.GetObject<IProducer>(ProducerName);
      producer.Publish(queue, message);
      var connection = (RedisConnection)producer.Connection;
      var actualMessage = connection.Multiplexer.GetDatabase().ListLeftPop(queue.Name);
      Assert.That(actualMessage.ToString(), Is.EqualTo(message));
    }
  }
}
