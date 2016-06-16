using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Tests.UtilTests;
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
      var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, 1);

      var channel = objectFactory.GetObject<RedisChannel>("myChannel");

      const int expectedConcurrency = 5;

      Assert.NotNull(channel);
      Assert.NotNull(channel.MessageQueue);
      Assert.NotNull(channel.DeadLetterQueue);
      Assert.NotNull(channel.PoisonQueue);
      Assert.NotNull(channel.MessageConverter);
      Assert.NotNull(channel.DefaultErrorHandler);

      Assert.AreEqual(expectedConcurrency, channel.Concurrency);
    }
  }
}