using MessageQueue.Contracts;
using MessageQueue.Contracts.Producer;
using Newtonsoft.Json;
using NUnit.Framework;
using RedisMessaging.Util;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class RedisMessagingImplementationTests
  {

    //private IApplicationContext _container;
    private IProducer _producer;
    private IContainer _consumer;

    [SetUp]
    public void Init()
    {
      //_container = ContextRegistry.GetContext();
      _producer = ServiceLocator.GetService<IProducer>("MyProducer");
      _consumer = ServiceLocator.GetService<IContainer>("MyContainer");
    }

    [TearDown]
    public void Dispose()
    {
      var connection = (RedisConnection)_producer.Connection;
      connection.Disconnect();
      connection = (RedisConnection)_consumer.Connection;
      connection.Disconnect();
    }

    [Test]
    public void RedisMessaging_DIInitTest()
    {
      _producer.Connection.Connect();
      _consumer.Init();
      Assert.IsTrue(_producer.Connection.IsConnected);
      Assert.IsTrue(_consumer.Connection.IsConnected);
    }

    [Test]
    public void RedisMessaging_LoadTest()
    {
      const int maxMessage = 5000;
      _producer.Connection.Connect();
      for (int i = 0; i < maxMessage; i++)
      {
        _producer.Publish(CreateBasicMessage(i, "hey hey hey"));
      }
      Assert.IsTrue(1 == 1);
    }

    [Test]
    public void RedisMessaging_UnloadTest()
    {
      _consumer.Init();
      var conn = (RedisConnection)_consumer.Connection;
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      {
        //do nothing
      }
      Assert.IsTrue(1 == 1);
    }

    public static string CreateBasicMessage(int number, string message)
    {
      KeyValuePair<string, object> kvp = new KeyValuePair<string, object>("Basic:"+number, message);
      return JsonConvert.SerializeObject(kvp);
    }

  }
}
