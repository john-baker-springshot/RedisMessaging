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

    private IApplicationContext _container;



    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [TearDown]
    public void Dispose()
    {
    }

    [Test]
    public void RedisMessaging_DIInitTest()
    {
      var _producer = ServiceLocator.GetService<IProducer>("MyProducer");
      var _consumer = ServiceLocator.GetService<IContainer>("MyContainer");
      _producer.Connection.Connect();
      _consumer.Init();
      Assert.IsTrue(_producer.Connection.IsConnected);
      Assert.IsTrue(_consumer.Connection.IsConnected);
    }

    [Test, MaxTime(10000)]
    public void RedisMessaging_LoadTest()
    {
      const int maxMessage = 10000;
      var _producer = ServiceLocator.GetService<IProducer>("MyProducer");
      _producer.Connection.Connect();
      for (int i = 0; i < maxMessage; i++)
      {
        _producer.Publish(CreateBasicMessage(i, "hey hey hey"));
      }

      //_consumer.Init();
      //var conn = (RedisConnection)_consumer.Connection;
      //while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      //{
      //  //do nothing
      //}
      //Assert.IsTrue(1 == 1);
      //Assert.IsTrue(1 == 1);
    }

    [Test, MaxTime(280000)]
    public void RedisMessaging_UnloadTest()
    {
      var _consumer = ServiceLocator.GetService<IContainer>("MyContainer");
      _consumer.Init();
      var conn = (RedisConnection)_consumer.Connection;
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
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
