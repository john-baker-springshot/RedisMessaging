using System;
using System.Xml;
using RedisMessaging.Consumer;
using RedisMessaging.Errors;
using RedisMessaging.Util;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Config
{
  public class RedisChannelParser : AbstractSingleObjectDefinitionParser
  {
    private static readonly string ListenerElementName = "listener";

    #region Overrides of AbstractSingleObjectDefinitionParser

    /// <summary>
    /// Gets the type of the object corresponding to the supplied XmlElement.
    /// </summary>
    /// <remarks>
    /// Note that, for application classes, it is generally preferable to override
    /// <code>
    /// GetObjectTypeName
    /// </code>
    ///  instad, in order to avoid a direct
    ///             dependence on the object implementation class.  The ObjectDefinitionParser
    ///             and its IXmlObjectDefinitionParser (namespace parser) can be used within an
    ///             IDE add-in then, even if the application classses are not available in the add-ins
    ///             AppDomain.
    /// </remarks>
    /// <param name="element">The element.</param>
    /// <returns>
    /// The Type of the class that is being defined via parsing the supplied
    ///             Element.
    /// </returns>
    protected override Type GetObjectType(XmlElement element)
    {
      return typeof(RedisChannel);
    }

    /// <summary>
    /// Parse the supplied XmlElement and populate the supplied ObjectDefinitionBuilder as required.
    /// </summary>
    /// <remarks>
    /// The default implementation delegates to the 
    /// <code>
    /// DoParse
    /// </code>
    ///  version without 
    ///             ParameterContext argument.
    /// </remarks>
    /// <param name="element">The element.</param><param name="parserContext">The parser context.</param><param name="builder">The builder used to define the 
    /// <code>
    /// IObjectDefinition
    /// </code>
    /// .</param>
    protected override void DoParse(XmlElement element, ParserContext parserContext, ObjectDefinitionBuilder builder)
    {
      const string messageQueuePropName = nameof(RedisChannel.MessageQueue);
      const string deadLetterQueuePropName = nameof(RedisChannel.DeadLetterQueue);
      const string poisonQueuePropName = nameof(RedisChannel.PoisonQueue);
      const string msgConverterPropName = nameof(RedisChannel.MessageConverter);
      const string errorHandlerPropName = nameof(RedisChannel.DefaultErrorHandler);

      var parentId = ResolveId(element, builder.ObjectDefinition, parserContext);

      if (!element.IsPropertyDefined(errorHandlerPropName))
      {
        var deadLetterErrHandlerObject = new RootObjectDefinition(typeof(DeadLetterErrorHandler));
        deadLetterErrHandlerObject.PropertyValues.Add(nameof(DeadLetterErrorHandler.Channel), new RuntimeObjectReference(parentId));
        builder.AddPropertyValue(errorHandlerPropName, deadLetterErrHandlerObject);
      }
      else
      {
        NamespaceUtils.SetPropertyIfAttributeOrElementDefined(element, parserContext, builder, errorHandlerPropName);
      }

      NamespaceUtils.CheckPresenceRule(element, parserContext, ListenerElementName);
      NamespaceUtils.CheckPresenceRule(element, parserContext, messageQueuePropName);
      NamespaceUtils.CheckPresenceRule(element, parserContext, msgConverterPropName);

      NamespaceUtils.CheckAmbiguityRule(element, parserContext, messageQueuePropName);
      NamespaceUtils.CheckAmbiguityRule(element, parserContext, deadLetterQueuePropName);
      NamespaceUtils.CheckAmbiguityRule(element, parserContext, poisonQueuePropName);
      NamespaceUtils.CheckAmbiguityRule(element, parserContext, msgConverterPropName);
      NamespaceUtils.CheckAmbiguityRule(element, parserContext, errorHandlerPropName);

      NamespaceUtils.SetPropertyIfAttributeOrElementDefined(element, parserContext, builder, messageQueuePropName);
      NamespaceUtils.SetPropertyIfAttributeOrElementDefined(element, parserContext, builder, deadLetterQueuePropName);
      NamespaceUtils.SetPropertyIfAttributeOrElementDefined(element, parserContext, builder, poisonQueuePropName);

      NamespaceUtils.SetPropertyIfAttributeOrElementDefined(element, parserContext, builder, msgConverterPropName);

      NamespaceUtils.SetValueIfAttributeDefined(builder, element, nameof(RedisChannel.Concurrency));

      NamespaceUtils.SetCollectionPropertyIfElementDefined(element, parserContext, builder, nameof(RedisChannel.Listeners), nameof(RedisListener.Channel), parentId);
      NamespaceUtils.SetCollectionPropertyIfElementDefined(element, parserContext, builder, nameof(RedisChannel.AdviceChain));
    }

    #endregion

    #region Overrides of AbstractObjectDefinitionParser

    /// <summary>
    /// Gets a value indicating whether an ID should be generated instead of read 
    ///             from the passed in XmlElement.
    /// </summary>
    /// <remarks>
    /// Note that this flag is about always generating an ID; the parser
    ///             won't even check for an "id" attribute in this case.
    /// </remarks>
    /// <value>
    /// <c>true</c> if should generate id; otherwise, <c>false</c>.
    /// </value>
    protected override bool ShouldGenerateId => false;

    /// <summary>
    /// Gets a value indicating whether an ID should be generated instead if the
    ///             passed in XmlElement does not specify an "id" attribute explicitly.
    /// </summary>
    /// <remarks>
    /// Disabled by default; subclasses can override this to enable ID generation
    ///             as fallback: The parser will first check for an "id" attribute in this case,
    ///             only falling back to a generated ID if no value was specified.
    /// </remarks>
    /// <value>
    /// <c>true</c> if should generate id if no value was specified; otherwise, <c>false</c>.
    /// </value>
    protected override bool ShouldGenerateIdAsFallback => true;

    #endregion
  }
}