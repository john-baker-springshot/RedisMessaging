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
using MessageQueue.Contracts;

namespace RedisMessaging.Tests.ConsumerTests
{
  [TestFixture]
  public class TestRedisChannel
  {

    private IApplicationContext _container;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
    }

    [Test]
    public void RedisChannel_DITest()
    {
      var testObject = _container.GetObject<IChannel>("MyChannel");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisChannel)));
    }

    [Test]
    public void RedisChannel_SubscribeTest()
    {
      //TODO
    }

  }
}
