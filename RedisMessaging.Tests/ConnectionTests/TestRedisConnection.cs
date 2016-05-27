using MessageQueue.Contracts;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using System;

namespace RedisMessaging.Tests.ConnectionTests
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
      var testObject = _container.GetObject<IConnection>("MyConnection");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisConnection_ConnectionTest()
    {
      const string connString = "localhost:6379";
      RedisConnection connection = new RedisConnection(connString);
      connection.Connect();
      Assert.IsTrue(connection.IsConnected);
      connection.Disconnect();
      Assert.IsFalse(connection.IsConnected);
    }

    [Test]
    public void RedisConnection_BadConnectionTest()
    {
      const string connString = "bad endpoint";
      RedisConnection connection = new RedisConnection(connString);
      Assert.That(() => connection.Connect(), Throws.Exception);
    }





  }
}
