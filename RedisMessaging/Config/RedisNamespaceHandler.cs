using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Config
{
  [NamespaceParser(Namespace = "http://www.springframework.net/schema/redis", 
    SchemaLocationAssemblyHint = typeof(RedisNamespaceHandler),
    SchemaLocation = "/RedisMessaging.Config/spring-redis-1.0.xsd")]
  public class RedisNamespaceHandler : NamespaceParserSupport
  {
    #region Overrides of NamespaceParserSupport

    /// <summary>
    /// Invoked by <see cref="T:Spring.Objects.Factory.Xml.NamespaceParserRegistry"/> after construction but before any
    ///             elements have been parsed.
    /// </summary>
    public override void Init()
    {
      this.RegisterObjectDefinitionParser("connection", new RedisConnectionParser());
    }

    #endregion
  }
}