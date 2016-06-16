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
      RegisterObjectDefinitionParser("connection", new RedisConnectionParser());

      RegisterObjectDefinitionParser("container", new RedisContainerParser());
      RegisterObjectDefinitionParser("channel", new RedisChannelParser());
      //RegisterObjectDefinitionParser("channel", new RedisChannelObjectParser());

      RegisterObjectDefinitionParser("sentinel", new RedisSentinelParser());

      var adviceParser = new RedisErrorAdviceParser();;
      RegisterObjectDefinitionParser("errorAdvice", adviceParser);
      RegisterObjectDefinitionParser("advice", adviceParser);

      var queueParser = new RedisQueueParser();
      RegisterObjectDefinitionParser("queue", queueParser);
      RegisterObjectDefinitionParser("messageQueue", queueParser);
      RegisterObjectDefinitionParser("deadLetterQueue", queueParser);
      RegisterObjectDefinitionParser("poisonQueue", queueParser);

      RegisterObjectDefinitionParser("typeMapper", new TypeMapperParser());
      RegisterObjectDefinitionParser("messageConverter", new RedisMessageConverterParser());
      RegisterObjectDefinitionParser("producer", new RedisProducerParser());

      RegisterObjectDefinitionParser("listener", new RedisListenerParser());
    }

    #endregion
  }
}