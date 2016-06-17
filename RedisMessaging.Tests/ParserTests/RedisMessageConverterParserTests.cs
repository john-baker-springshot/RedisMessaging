using NUnit.Framework;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisMessageConverterParserTests
  {
    private readonly string ConfigConventionPrefix = "MessageConverter";

    [Test]
    public void TestWithTypeMapperAsAttributeReference()
    {
      const int expectedKnownTypesCount = 2;

      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 1);

      var msgConverter = objectFactory.GetObject<JsonMessageConverter>("msgConverter");

      Assert.NotNull(msgConverter);
      Assert.IsTrue(msgConverter.CreateMessageIds);
      Assert.NotNull(msgConverter.TypeMapper);
      Assert.AreEqual(expectedKnownTypesCount, msgConverter.TypeMapper.Types.Count);
    }

    [Test]
    public void TestWithInlineTypeMapper()
    {
      const int expectedKnownTypesCount = 2;

      var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 2);

      var msgConverter = objectFactory.GetObject<JsonMessageConverter>("msgConverter");

      Assert.NotNull(msgConverter);
      Assert.IsTrue(msgConverter.CreateMessageIds);
      Assert.NotNull(msgConverter.TypeMapper);
      Assert.AreEqual(expectedKnownTypesCount, msgConverter.TypeMapper.Types.Count);
    }

    [Test]
    public void TestWithTypeMapperInlineAndRefAttribute()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 3);
      });
    }

    [Test]
    public void TestWithTypeMapperAttributeBrokenRef()
    {
      Assert.Throws<ObjectCreationException>(() =>
      {
        var objectFactory = ParserTestsHelper.LoadConfig(ConfigConventionPrefix, 4);

        objectFactory.GetObject<JsonMessageConverter>("msgConverter");
      });
    }
  }
}