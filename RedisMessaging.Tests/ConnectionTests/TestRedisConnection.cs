using MessageQueue.Contracts.ConnectionBase;
using NUnit.Framework;
using RedisMessaging.ConnectionBase;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class TestRedisConnection
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisConnection_DI_Test()
    {
      var testObject = _container.GetObject<IConnection>();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisConnection_AddServerTest()
    {
      var RedisConnection = new RedisConnection();
      var server = new RedisServer("localhost");
      RedisConnection.AddServer(server);
      Assert.IsTrue(RedisConnection.Servers.Contains(server) );
    }

    [Test]
    public void RedisConnection_AddNullServerTest()
    {
      var RedisConnection = new RedisConnection();
      RedisServer server = null;
      RedisConnection.AddServer(server);
      Assert.IsFalse(RedisConnection.Servers.Contains(server));
    }

  }
}
