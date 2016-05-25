using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.ConnectionBase;
using RedisMessaging.Consumer;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Tests.ConsumerTests
{
 [TestFixture]
  public class TestRedisContainer
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisContainer_DITest()
    {
      var testObject = _container.GetObject<IContainer>();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisContainer)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisContainer_CtorTest()
    {
      const string endpoint = "localhost";
      RedisConnection conn = new RedisConnection();
      conn.AddServer(new RedisServer(endpoint));
      RedisContainer container = new RedisContainer(conn);
      Assert.That(container.Connection, Is.EqualTo(conn));
      Assert.That(container.Connection.Servers.Count, Is.EqualTo(1));
      Assert.That(container.Connection.Servers[0].Endpoint, Is.EqualTo(endpoint));
    }


    [Test]
    public void RedisContainer_AddChannelTest()
    {
      //RedisConnection conn = new RedisConnection();
      //RedisContainer container = new RedisContainer(conn);
      //RedisChannel channel = new RedisChannel();

      //container.AddChannel(channel);
      //Assert.IsTrue(container.Channels.Contains(channel));
    }

    [Test]
    public void RedisContainer_InitTest()
    {
      //RedisConnection conn = new RedisConnection();
      //RedisContainer container = new RedisContainer(conn);
      //RedisChannel channel = new RedisChannel();
      //container.AddChannel(channel);
      ////this isnt ready to be tested, especially on a blank channel
      ////TODO: container.Init();
      //Assert.IsTrue(1 == 1);
    }

  }
}
