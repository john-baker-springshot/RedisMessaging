using System;
using System.Xml;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Config
{
  public class ErrorHandlerObjectParser : IObjectDefinitionParser
  {
    #region Implementation of IObjectDefinitionParser

    /// <summary>
    /// Parse the specified XmlElement and register the resulting
    ///             ObjectDefinitions with the <see cref="P:Spring.Objects.Factory.Xml.ParserContext.Registry"/> IObjectDefinitionRegistry
    ///             embedded in the supplied <see cref="T:Spring.Objects.Factory.Xml.ParserContext"/>
    /// </summary>
    /// <remarks>
    /// <p>This method is never invoked if the parser is namespace aware
    ///             and was called to process the root node.
    ///             </p>
    /// </remarks>
    /// <param name="element">The element to be parsed.
    ///             </param><param name="parserContext">The object encapsulating the current state of the parsing process. 
    ///             Provides access to a IObjectDefinitionRegistry
    ///             </param>
    /// <returns>
    /// The primary object definition.
    /// </returns>
    public IObjectDefinition ParseElement(XmlElement element, ParserContext parserContext)
    {
      var type = Type.GetType(element.GetAttribute("type"), true, false);

      return new RootObjectDefinition(type);
    }

    #endregion
  }
}