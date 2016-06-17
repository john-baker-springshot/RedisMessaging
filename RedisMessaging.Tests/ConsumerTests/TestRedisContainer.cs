using MessageQueue.Contracts;
using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.Consumer;
using Spring.Context;
using Spring.Context.Support;
using System.Linq;
using System.Runtime.InteropServices;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory.Xml;
using StackExchange.Redis;

namespace RedisMessaging.Tests.ConsumerTests
{
  [TestFixture]
  public class TestRedisContainer
  {
    private XmlObjectFactory _objectFactory;
    internal const string ContainerName = "myContainer";

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
    public void RedisContainer_DITest()
    {
      var testObject = _objectFactory.GetObject<IContainer>(ContainerName);
      testObject.Init();

      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisContainer)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
      Assert.That(testObject.Channels.First().GetType(), Is.EqualTo(typeof(RedisChannel)));
      Assert.That(testObject.Channels.Count(), Is.EqualTo(5));
    }

    [Test]
    public void RedisContainer_CtorTest()
    {
      const string endpoint = "localhost:6379";
      var conn = new RedisConnection(endpoint);
      var container = new RedisContainer(conn);

      Assert.That(container.Connection, Is.EqualTo(conn));
    }

    [Test]
    public void RedisContainer_InitTest()
    {
      var testObject = _objectFactory.GetObject<IContainer>(ContainerName);
      testObject.Init();
      Assert.IsTrue(testObject.Connection.IsConnected);
      Assert.IsTrue(testObject.Channels.First().IsSubscribed);
    }
  }
}
