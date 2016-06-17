using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.Consumer;
using RedisMessaging.Tests.UtilTests;
using Spring.Context;
using Spring.Context.Support;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ConsumerTests
{
  [TestFixture]
  public class TestRedisListener
  {

    private XmlObjectFactory _objectFactory;
    private const string ListenerName = "myListener";
    private static bool _testValue = true;

    [OneTimeSetUp]
    public void Init()
    {
      _objectFactory = ParserTestsHelper.LoadMessagingConfig();
      _testValue = true;
    }

    [OneTimeTearDown]
    public void Dispose()
    {
      _objectFactory.Dispose();
    }

    [Test]
    public void RedisListener_DITest()
    {
      var testObject = _objectFactory.GetObject<IListener>(ListenerName);
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisListener)));
    }

    [Test]
    public void RedisListener_InternalHandlerTest()
    {
      var listener = _objectFactory.GetObject<IListener>(ListenerName) as RedisListener;
      Assert.That(listener.HandlerType, Is.EqualTo(typeof(TestMessageHandler)));
      _testValue = true;

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

  public class TestMessageHandler
  {
    public void HandleMessage(object m)
    {
      TestRedisListener.UpdateTestValueForHandlerTest();
    }
  }

}
