using NUnit.Framework.Internal;
using RedisMessaging.Config;
using Spring.Core.IO;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.UtilTests
{
  public static class ParserTestsHelper
  {
    public static XmlObjectFactory LoadConfig(string configConventionPrefix, int configId)
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));

      var resourceName = $"assembly://RedisMessaging.Tests/RedisMessaging.Tests.Configs.{configConventionPrefix}/{configConventionPrefix}-{configId}.config";

      var resource = new AssemblyResource(resourceName);
      return new XmlObjectFactory(resource);
    }

    public static XmlObjectFactory LoadMessagingConfig()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));

      const string resourceName = "assembly://RedisMessaging.Tests/RedisMessaging.Tests.Configs/Messaging.config";

      var resource = new AssemblyResource(resourceName);
      return new XmlObjectFactory(resource);
    }
  }
}