using NUnit.Framework;
using RedisMessaging.Producer;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisProducerParserTests
  {
    private readonly string ConfigConventionPrefix = "Producer";

    [Test]
    public void TestWithAttributeRefs()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 1);

      var producer = objectFactory.GetObject<RedisProducer>("myProducer");

      var expectedConnection = objectFactory.GetObject<RedisConnection>("strongConnection");
      var expectedQueue = objectFactory.GetObject<RedisQueue>("myQueue");

      Assert.NotNull(producer);
      Assert.IsNotNull(producer.Connection);
      Assert.IsNotNull(producer.Queue);
      Assert.AreEqual(expectedConnection, producer.Connection);
      Assert.AreEqual(expectedQueue, producer.Queue);
    }

    [Test]
    public void TestWithNoQueueProvided()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 2);

      var producer = objectFactory.GetObject<RedisProducer>("myProducer");

      var expectedConnection = objectFactory.GetObject<RedisConnection>("strongConnection");

      Assert.NotNull(producer);
      Assert.IsNotNull(producer.Connection);
      Assert.IsNull(producer.Queue);
      Assert.AreEqual(expectedConnection, producer.Connection);
    }

    [Test]
    [TestCase(3)]
    [TestCase(4)]
    public void TestWithBrokenConnectionRefAttribute(int configId)
    {
      Assert.Throws<ObjectCreationException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);

        objectFactory.GetObject<RedisProducer>("myProducer");
      });
    }

    [Test]
    public void TestWithMissingConnection()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 5);
      });
    }

    [Test]
    [TestCase(6)]
    [TestCase(7)]
    public void TestWithAmbiguousConfiguration(int configId)
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);
      });
    }

    [Test]
    public void TestWithInlineConfiguration()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 8);

      var producer = objectFactory.GetObject<RedisProducer>("myProducer");

      Assert.NotNull(producer);
      Assert.IsNotNull(producer.Connection);
      Assert.IsNotNull(producer.Queue);

    }

  }
}