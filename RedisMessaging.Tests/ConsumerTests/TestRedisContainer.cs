using MessageQueue.Contracts;
using MessageQueue.Contracts.Consumer;
using NUnit.Framework;
using RedisMessaging.Consumer;
using Spring.Context;
using Spring.Context.Support;
using System.Linq;
using StackExchange.Redis;

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
      testObject.Init();
      Assert.IsNotNull(testObject);
      Assert.That(testObject.GetType(), Is.EqualTo(typeof(RedisContainer)));
      Assert.That(testObject.Connection.GetType(), Is.EqualTo(typeof(RedisConnection)));
      Assert.That(testObject.Channels.First().GetType(), Is.EqualTo(typeof(RedisChannel)));
      Assert.That(testObject.Channels.Count, Is.EqualTo(5));
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

   //[Test]
   public void RedisContainer_SentinelTest()
   {
      var connection = _container.GetObject<IConnection>("MyConnection") as RedisConnection;
      //RedisQueueSentinel sent = new RedisQueueSentinel(connection, 10);


      var container = _container.GetObject<IContainer>("MyContainer");
      container.Init();
      var channel = container.Channels.First() as RedisChannel;
      var redisConnection = container.Connection as RedisConnection;
      IConnectionMultiplexer redis = redisConnection.Multiplexer;

      //push messages to the ProcessingQueue
      redis.GetDatabase().ListLeftPush(channel.ProcessingQueue.Name, "test");

      //wait 10 seconds for "timeout" time to occur
      System.Threading.Thread.Sleep(12000);


      //should requeue the messages
      //BUT if the container is init'd, than the Channels are actively subscribing
      //so how can I assert that the Sentinel is doing what its supposed to
    }

  }
}
