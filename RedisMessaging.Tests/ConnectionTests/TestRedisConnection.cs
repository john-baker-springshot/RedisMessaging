using MessageQueue.Contracts;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using System;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ConnectionTests
{
  [TestFixture]
  public class TestRedisConnection
  {

    private XmlObjectFactory _objectFactory;

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
    public void RedisConnection_DI_Test()
    {
      var testObject = _objectFactory.GetObject<IConnection>("strongConnection");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }

    [Test]
    public void RedisConnection_ConnectionTest()
    {
      const string connString = "localhost:6379";
      var connection = new RedisConnection(connString);
      connection.Connect();
      Assert.IsTrue(connection.IsConnected);
      connection.Dispose();
      Assert.IsFalse(connection.IsConnected);
    }

    [Test]
    public void RedisConnection_BadConnectionTest()
    {
      const string connString = "bad endpoint";
      RedisConnection connection; 
      Assert.That(() => connection = new RedisConnection(connString), Throws.Exception);
    }
  }
}
