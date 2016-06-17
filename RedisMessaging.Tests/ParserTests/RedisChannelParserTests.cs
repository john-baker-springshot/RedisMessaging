using System.Linq;
using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Errors;
using RedisMessaging.Tests.UtilTests;
using RedisMessaging.Util;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisChannelParserTests
  {
    private readonly string ConfigConventionPrefix = "Channel";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }


    [Test]
    public void TestWithInlineConfig()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 1);

      var channel = objectFactory.GetObject<RedisChannel>("myChannel");

      const int expectedConcurrency = 5;
      const int expectedListenerCount = 1;
      const int expectedAdviceLength = 2;

      Assert.NotNull(channel);
      Assert.NotNull(channel.MessageQueue);
      Assert.NotNull(channel.DeadLetterQueue);
      Assert.NotNull(channel.PoisonQueue);
      Assert.NotNull(channel.MessageConverter);
      Assert.NotNull(channel.DefaultErrorHandler);

      Assert.AreEqual(expectedConcurrency, channel.Concurrency);
      Assert.AreEqual(expectedListenerCount, channel.Listeners.Count());
      Assert.AreEqual(expectedAdviceLength, channel.AdviceChain.Count());
    }

    [Test]
    public void TestWithRefObjects()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 2);

      var channel = objectFactory.GetObject<RedisChannel>("myChannel");

      const int expectedConcurrency = 5;
      const int expectedListenerCount = 1;

      Assert.NotNull(channel);
      Assert.NotNull(channel.MessageQueue);
      Assert.NotNull(channel.DeadLetterQueue);
      Assert.NotNull(channel.PoisonQueue);
      Assert.NotNull(channel.MessageConverter);
      Assert.NotNull(channel.DefaultErrorHandler);

      Assert.AreEqual(expectedConcurrency, channel.Concurrency);
      Assert.AreEqual(expectedListenerCount, channel.Listeners.Count());
    }

    [Test]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    public void TestDefenseAgainstBrokenRefs(int configId)
    {
      Assert.Throws<ObjectCreationException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);

        objectFactory.GetObject<RedisChannel>("myChannel");
      });
    }

    [Test]
    [TestCase(9, nameof(RedisChannel.MessageQueue))]
    [TestCase(10, nameof(RedisChannel.MessageConverter))]
    public void TestDefenseAgainstMissingRefs(int configId, string propertyName)
    {
      var exception = Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);

        objectFactory.GetObject<RedisChannel>("myChannel");
      });

      var attrName = propertyName.ToCamelCase();
      var expectedErrorMessage = $"Either {attrName} attribute or the {attrName} element (but not both) should be defined.";
      Assert.AreEqual(expectedErrorMessage, exception.GetBaseException().Message);
    }

    [Test]
    [TestCase(11, nameof(RedisChannel.MessageQueue))]
    [TestCase(12, nameof(RedisChannel.MessageConverter))]
    [TestCase(13, nameof(RedisChannel.DefaultErrorHandler))]
    [TestCase(14, nameof(RedisChannel.DeadLetterQueue))]
    [TestCase(15, nameof(RedisChannel.PoisonQueue))]
    public void TestDefenseAgainstAmbiguousConfig(int configId, string propertyName)
    {
      var exception = Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, configId);

        objectFactory.GetObject<RedisChannel>("myChannel");
      });

      var attrName = propertyName.ToCamelCase();
      var expectedErrorMessage = $"The {attrName} attribute and the {attrName} element, both cannot be defined at the same time.";
      Assert.AreEqual(expectedErrorMessage, exception.GetBaseException().Message);
    }

    [Test]
    public void TestDefaultErrorHandlerAutoAssignedWhenNoneConfigured()
    {
      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 8);

      var channel = objectFactory.GetObject<RedisChannel>("myChannel");

      Assert.NotNull(channel);
      Assert.NotNull(channel.DefaultErrorHandler);
      Assert.IsInstanceOf<DeadLetterErrorHandler>(channel.DefaultErrorHandler);
    }
  }
}