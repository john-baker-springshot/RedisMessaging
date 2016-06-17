using MessageQueue.Contracts;
using MessageQueue.Contracts.Producer;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class RedisMessagingImplementationTests
  {
    private XmlObjectFactory _objectFactory;

    private const string ProducerName = "myProducer";
    private const string ConsumerName = "myContainer";

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
    public void RedisMessaging_DIInitTest()
    {
      var producer = _objectFactory.GetObject<IProducer>(ProducerName);
      var consumer = _objectFactory.GetObject<IContainer>(ConsumerName);
      consumer.Init();
      Assert.IsTrue(producer.Connection.IsConnected);
      Assert.IsTrue(consumer.Connection.IsConnected);
    }

    [Test, MaxTime(10000)]
    public void RedisMessaging_LoadTest()
    {
      const int maxMessage = 100000;
      var producer = _objectFactory.GetObject<IProducer>(ProducerName);

      for (int i = 0; i < maxMessage; i++)
      {
        producer.Publish(CreateBasicMessage(i, "hey hey hey"));
      }
    }

    [Test, MaxTime(20000)]
    public void RedisMessaging_UnloadTest()
    {
      var consumer = _objectFactory.GetObject<IContainer>(ConsumerName);
      consumer.Init();

      var conn = (RedisConnection)consumer.Connection;
      var queueName = consumer.Channels.First().MessageQueue.Name;

      while (conn.Multiplexer.GetDatabase().ListLength(queueName) > 0)
      {
        //do nothing
      }
      System.Threading.Thread.Sleep(5000);
      //Assert.IsTrue(1 == 1);
    }

    public static string CreateBasicMessage(int number, string message)
    {
      KeyValuePair<string, BasicMessage> kvp = new KeyValuePair<string, BasicMessage>("Basic:"+number, new BasicMessage(message));
      return JsonConvert.SerializeObject(kvp);
    }

  }
}
