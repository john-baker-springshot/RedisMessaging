using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace RedisMessaging.Config
{
  public abstract class NamespaceParserSupport : INamespaceParser
  {
    private readonly IDictionary<string, IObjectDefinitionParser> objectParsers = new Dictionary<string, IObjectDefinitionParser>();

    #region IXmlObjectDefinitionParser Members

    /// <summary>
    /// Invoked by <see cref="NamespaceParserRegistry"/> after construction but before any
    /// elements have been parsed.
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Parses an element under the root node, typically
    /// an object definition or import statement.
    /// </summary>
    /// <param name="element">
    /// The element to be parsed.
    /// </param>
    /// <param name="parserContext">
    /// The parser context.
    /// </param>
    /// <returns>
    /// The number of object defintions created from this element.
    /// </returns>
    public virtual IObjectDefinition ParseElement(XmlElement element, ParserContext parserContext)
    {
      return FindParserForElement(element, parserContext).ParseElement(element, parserContext);
    }


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
    public ObjectDefinitionHolder Decorate(XmlNode node, ObjectDefinitionHolder definition,
                                           ParserContext parserContext)
    {
      return null;
    }

    private IObjectDefinitionParser FindParserForElement(XmlElement element, ParserContext parserContext)
    {
      IObjectDefinitionParser parser;
      if (!objectParsers.TryGetValue(element.LocalName, out parser))
      {
        parserContext.ReaderContext.ReportException(element, "unknown object name", "Cannot locate IObjectDefinitionParser for element ["
            + element.LocalName + "]");
      }
      return parser;

    }

    /// <summary>
    /// Register the specified <see cref="IObjectDefinitionParser"/> for the given <paramref name="elementName"/>
    /// </summary>
    protected virtual void RegisterObjectDefinitionParser(string elementName, IObjectDefinitionParser parser)
    {
      AssertUtils.ArgumentNotNull(elementName, "elementName");
      AssertUtils.ArgumentNotNull(parser, "parser");
      objectParsers[elementName] = parser;
    }

    #endregion
  }
}
