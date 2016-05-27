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
    private static bool _testValue = true;

    [SetUp]
    public void Init()
    {
      _container = ContextRegistry.GetContext();
      _testValue = true;
    }

    [Test]
    public void RedisListener_DITest()
    {
      var testObject = _container.GetObject<IListener>("MyListener");
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisListener)));
    }

    [Test]
    public void RedisListener_InternalHandlerTest()
    {
      var listener = _container.GetObject<IListener>("MyListener") as RedisListener;
      listener.MessageHandler = new TestMessageHandler();
      Assert.That(listener.MessageHandler.GetType(), Is.EqualTo(typeof(TestMessageHandler)));
      listener.InternalHandlerAsync("handle it!");
      //need to sleep for async thread to catch up
      //System.Threading.Thread.Sleep(3000);
      Assert.IsFalse(_testValue);
    }

    public static void UpdateTestValueForHandlerTest()
    {
      _testValue = !_testValue;
    }
  }

  public class TestMessageHandler : IMessageHandler
  {
    public void HandleMessage(object m)
    {
      TestRedisListener.UpdateTestValueForHandlerTest();
    }
  }

}
