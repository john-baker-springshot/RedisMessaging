using System;
using MessageQueue.Contracts;
using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class TypeMapperParsetTests
  {
    private readonly string ConfgiConventionPrefix = "TypeMapper";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }

    [Test]
    public void TestTypeMapperWithValidConfig()
    {
      const int expectedMapCount = 2;
      var basicMessageType = typeof (BasicMessage);
      const string basicMessageTypeKey = "Basic";

      var objectFactory = ParserTestsHelper.LoadContext(ConfgiConventionPrefix, 1);

      var mapper = objectFactory.GetObject<TypeMapper>("myTypeMapper");

      Assert.NotNull(mapper);
      Assert.AreEqual(expectedMapCount, mapper.Types.Count);

      var actualType = mapper.GetTypeForKey(basicMessageTypeKey);
      Assert.AreEqual(basicMessageType, actualType);
    }

    [Test]
    public void TestTypeMapperConfigWithMissingKeys()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfgiConventionPrefix, 2);
      });
    }

    [Test]
    public void TestTypeMapperConfigWithMissingTypes()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfgiConventionPrefix, 3);
      });
    }

    [Test]
    public void TestTypeMapperConfigWithUnresolvableTypes()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfgiConventionPrefix, 4);
      });
    }
  }
}