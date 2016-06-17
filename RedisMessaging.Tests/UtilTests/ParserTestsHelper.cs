using NUnit.Framework.Internal;
using Spring.Core.IO;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.UtilTests
{
  public static class ParserTestsHelper
  {
    public static XmlObjectFactory LoadContext(string configConventionPrefix, int configId)
    {
      var resourceName = $"assembly://RedisMessaging.Tests/RedisMessaging.Tests.Configs.{configConventionPrefix}/{configConventionPrefix}-{configId}.config";

      var resource = new AssemblyResource(resourceName);
      return new XmlObjectFactory(resource);
    }
  }
}