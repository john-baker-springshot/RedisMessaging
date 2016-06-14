using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisSentinelParserTests
  {
    private readonly string ConfigConventionPrefix = "Sentinel";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }

    [Test]
    [TestCase(1, 60, 1000)]
    [TestCase(3, 300, 10)]
    public void TestLoadingValidSentinelConfig(int configId, int expectedMessageTimeout, int expectedInterval)
    {
      var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, configId);

      var sentinel = objectFactory.GetObject<RedisQueueSentinel>("sentinel");

      Assert.NotNull(sentinel);
      Assert.NotNull(sentinel.Connection);
      Assert.AreEqual(expectedMessageTimeout, sentinel.MessageTimeout);
      Assert.AreEqual(expectedInterval, sentinel.Interval);
    }

    [Test]
    public void TestInvalidSentinelConfig()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfigConventionPrefix, 2);
      });
    }
  }
}