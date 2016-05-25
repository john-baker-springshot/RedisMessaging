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

namespace RedisMessaging.Tests.ConnectionTests
{
  [TestFixture]
  public class TestRedisServer
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisServer_DI_Test()
    {
      const string endpoint = "localhost";
      var testObject = _container.GetObject<IServer>();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisServer)));
      Assert.That(testObject.Endpoint, Is.EqualTo(endpoint));
    }

    [Test]
    public void RedisServer_CtorTest()
    {
      const string endpoint = "localhost";
      RedisServer server = new RedisServer(endpoint);
      Assert.That(server.Endpoint, Is.EqualTo(endpoint));
    }

  }
}
