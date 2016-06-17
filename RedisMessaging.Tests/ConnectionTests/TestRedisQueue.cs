using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using MessageQueue.Contracts;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ConnectionTests
{
  [TestFixture]
  public class TestRedisQueue
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
    public void RedisQueue_DI_Test()
    {
      const string name = "myRedisQ";
      const int ttl = 10000;
      var testObject = _objectFactory.GetObject<IQueue>("myQueue");
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
