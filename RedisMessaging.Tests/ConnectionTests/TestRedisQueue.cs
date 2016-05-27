using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using MessageQueue.Contracts;

namespace RedisMessaging.Tests.ConnectionTests
{
  [TestFixture]
  public class TestRedisQueue
  {
    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisQueue_DI_Test()
    {
      const string name = "MessageQueue";
      const int ttl = 0;
      var testObject = _container.GetObject<IQueue>("MyMessageQueue");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisQueue)));
      Assert.That(testObject.Name, Is.EqualTo(name));
      Assert.That(testObject.TTL, Is.EqualTo(ttl));
    }

    [Test]
    public void RedisQueue_CtorTest()
    {
      const string name = "defaultQueue";
      const int ttl = 0;
      RedisQueue q = new RedisQueue(name, ttl);
      Assert.That(q.Name, Is.EqualTo(name));
      Assert.That(q.TTL, Is.EqualTo(ttl));
    }
  }
}
