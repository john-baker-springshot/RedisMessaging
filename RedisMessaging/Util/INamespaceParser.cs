using System.Xml;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Util
{
  public interface INamespaceParser
  {

    /// <summary>
    /// Invoked by <see cref="NamespaceParserRegistry"/> after construction but before any
    /// elements have been parsed.
    /// </summary>
    void Init();


    /// <summary>
    /// Parse the specified element and register any resulting
    /// IObjectDefinitions with the IObjectDefinitionRegistry that is
    /// embedded in the supplied ParserContext.
    /// </summary>
    /// <remarks>
    /// Implementations should return the primary IObjectDefinition
    /// that results from the parse phase if they wish to used nested
    /// inside (for example) a <code>&lt;property&gt;</code> tag.
    /// <para>Implementations may return null if they will not
    /// be used in a nested scenario.
    /// </para>
    /// </remarks>
    /// <param name="element">The element to be parsed into one or more IObjectDefinitions</param>
    /// <param name="parserContext">The object encapsulating the current state of the parsing
    /// process.</param>
    /// <returns>
    /// The primary IObjectDefinition (can be null as explained above)
    /// </returns>
    IObjectDefinition ParseElement(XmlElement element, ParserContext parserContext);


    /// <summary>
    /// Parse the specified XmlNode and decorate the supplied ObjectDefinitionHolder,
    /// returning the decorated definition.
    /// </summary>
    /// <remarks>The XmlNode may either be an XmlAttribute or an XmlElement, depending on
    /// whether a custom attribute or element is being parsed.
    /// <para>Implementations may choose to return a completely new definition,
    /// which will replace the original definition in the resulting IApplicationContext/IObjectFactory.
    /// </para>
    /// <para>The supplied ParserContext can be used to register any additional objects needed to support
    /// the main definition.</para>
    /// </remarks>
    /// <param name="node">The source element or attribute that is to be parsed.</param>
    /// <param name="definition">The current object definition.</param>
    /// <param name="parserContext">The object encapsulating the current state of the parsing
    /// process.</param>
    /// <returns>The decorated definition (to be registered in the IApplicationContext/IObjectFactory),
    /// or simply the original object definition if no decoration is required.  A null value is strickly
    /// speaking invalid, but will leniently treated like the case where the original object definition
    /// gets returned.</returns>
    ObjectDefinitionHolder Decorate(XmlNode node, ObjectDefinitionHolder definition, ParserContext parserContext);
  }
}
