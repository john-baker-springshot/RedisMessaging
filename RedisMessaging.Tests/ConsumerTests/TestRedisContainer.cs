using MessageQueue.Contracts;
using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.Consumer;
using Spring.Context;
using Spring.Context.Support;
using System.Linq;

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
      var testObject = _container.GetObject<IContainer>("MyContainer");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisContainer)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
      Assert.That(testObject.Channels.First().GetType(), Is.EqualTo(typeof(RedisChannel)));
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
      var testObject = _container.GetObject<IContainer>("MyContainer");
      testObject.Init();
      Assert.IsTrue(testObject.Connection.IsConnected);
      Assert.IsTrue(testObject.Channels.First().IsSubscribed);     
    }

  }
}
