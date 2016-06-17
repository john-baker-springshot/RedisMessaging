using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisQueueParserTests
  {
    private readonly string ConfigConventionPrefix = "Queue";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }

    [Test]
    [TestCase(1, "MyTestQ", 1000)]
    [TestCase(3, "QWithDefaultTTL", 10000)]
    public void TestValidQueueConfig(int configId, string expectedQueueName, int expectedTtl)
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);

      var q = objectFactory.GetObject<RedisQueue>("queue1");

      Assert.NotNull(q);
      Assert.AreEqual(expectedQueueName, q.Name);
      Assert.AreEqual(expectedTtl, q.TTL);
    }

    [Test]
    public void TestMissingQueueNameTest()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 2);
      });
    }
  }
}