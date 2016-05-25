using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
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
  public class TestRedisListener
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisListener_DITest()
    {
      var testObject = _container.GetObject<IListener>();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisListener)));
    }

    [Test]
    public void RedisListener_InternalHandlerTest()
    {
      //TODO
    }

  }
}
