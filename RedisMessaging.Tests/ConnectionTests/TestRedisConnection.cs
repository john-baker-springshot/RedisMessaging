using MessageQueue.Contracts;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

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
      var testObject = _container.GetObject<IConnection>();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisConnection)));
    }
  }
}
