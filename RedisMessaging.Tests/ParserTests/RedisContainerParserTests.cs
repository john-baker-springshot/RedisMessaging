using System.Linq;
using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisContainerParserTests
  {
    private readonly string ConfigConventionPrefix = "Container";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }

    [Test]
    public void TestWithValidConfig()
    {
      var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, 1);

      var container = objectFactory.GetObject<RedisContainer>("myContainer");

      const int expectedChannelsCount = 1;
      var expectedConnection = objectFactory.GetObject<RedisConnection>("strongConnection");
      var expectedSentinel = objectFactory.GetObject<RedisQueueSentinel>("sentinel");

      Assert.NotNull(container);
      Assert.AreEqual(expectedChannelsCount, container.Channels.Count());
      Assert.AreEqual(expectedConnection, container.Connection);
      Assert.AreEqual(expectedSentinel, container.Sentinel);
      Assert.IsTrue(container.EnableSentinel);
    }

    [Test]
    public void TestThatContainerMustHaveOneOrMoreChannels()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfigConventionPrefix, 2);
      });
    }

    [Test]
    [TestCase(3)]
    [TestCase(4)]
    public void TestDefenseAgainstBrokenRefs(int configId)
    {
      Assert.Throws<ObjectCreationException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, configId);

        objectFactory.GetObject<RedisContainer>("myContainer");
      });
    }
  }
}