using System.Xml;
using RedisMessaging.Util;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace RedisMessaging.Config
{
  /// <summary>
  /// Namespace Utilities
  /// </summary>
  /// <author>Joe Fitzgerald (.NET)</author>
  public class NamespaceUtils
  {
    private static readonly string BASE_PACKAGE = "Spring.Messaging.Amqp.Rabbit.Config";
    private static readonly string REF_ATTRIBUTE = "ref";
    private static readonly string METHOD_ATTRIBUTE = "method";
    private static readonly string ORDER = "order";

    /// <summary>Sets the value if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>True if successful, else false.</returns>
    public static bool SetValueIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName, string propertyName)
    {
      if (element.IsAttributeDefined(attributeName))
      {
        var attributeValue = element.GetAttribute(attributeName);
        if (!string.IsNullOrWhiteSpace(attributeValue))
        {
          builder.AddPropertyValue(propertyName, new TypedStringValue(attributeValue));
          return true;
        }
      }
      return false;
    }

    /// <summary>Sets the value if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the attribute.</param>
    /// <returns>True if successful, else false.</returns>
    public static bool SetValueIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element,
      string propertyName)
    {
      return SetValueIfAttributeDefined(builder, element, propertyName.ToCamelCase(), propertyName);
    }

    /// <summary>Adds the constructor arg value. Should be used only for mandatory attributes or attributes with default values.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool AddConstructorArgValue(ObjectDefinitionBuilder builder, XmlElement element, string attributeName)
    {
      return AddConstructorArgValueIfAttributeDefined(builder, element, attributeName);
    }

    /// <summary>Adds the constructor arg value if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool AddConstructorArgValueIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName)
    {
      var value = element.GetAttribute(attributeName);
      if (!string.IsNullOrWhiteSpace(value))
      {
        builder.AddConstructorArg(new TypedStringValue(value));
        return true;
      }

      return false;
    }

    /// <summary>Adds the constructor arg boolean value if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
    public static void AddConstructorArgBooleanValueIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName, bool defaultValue)
    {
      var value = element.GetAttribute(attributeName);
      if (!string.IsNullOrWhiteSpace(value))
      {
        builder.AddConstructorArg(new TypedStringValue(value));
      }
      else
      {
        builder.AddConstructorArg(defaultValue);
      }
    }

    /// <summary>Adds the constructor arg ref if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool AddConstructorArgRefIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName)
    {
      var value = element.GetAttribute(attributeName);
      if (!string.IsNullOrWhiteSpace(value))
      {
        builder.AddConstructorArgReference(value);
        return true;
      }

      return false;
    }

    /// <summary>Adds the constructor arg parent ref if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool AddConstructorArgParentRefIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName)
    {
      var value = element.GetAttribute(attributeName);
      if (!string.IsNullOrWhiteSpace(value))
      {
        var child = ObjectDefinitionBuilder.GenericObjectDefinition();
        child.RawObjectDefinition.ParentName = value;
        builder.AddConstructorArg(child.ObjectDefinition);
        return true;
      }

      return false;
    }

    /// <summary>Sets the reference if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool SetReferenceIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element, string attributeName, string propertyName)
    {
      var attributeValue = element.GetAttribute(attributeName);
      if (!string.IsNullOrWhiteSpace(attributeValue))
      {
        builder.AddPropertyReference(propertyName, attributeValue);
        return true;
      }

      return false;
    }

    /// <summary>Sets the reference if attribute defined.</summary>
    /// <param name="builder">The builder.</param>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool SetReferenceIfAttributeDefined(ObjectDefinitionBuilder builder, XmlElement element,
      string propertyName)
    {
      return SetReferenceIfAttributeDefined(builder, element, propertyName.ToCamelCase(), propertyName);
    }

    /// <summary>Creates the element description.</summary>
    /// <param name="element">The element.</param>
    /// <returns>The element description.</returns>
    public static string CreateElementDescription(XmlElement element)
    {
      var elementId = "'" + element.LocalName + "'";
      var id = element.GetAttribute("id");
      if (!string.IsNullOrWhiteSpace(id))
      {
        elementId += " with id='" + id + "'";
      }

      return elementId;
    }

    /// <summary>The parse inner object definition.</summary>
    /// <param name="element">The element.</param>
    /// <param name="parserContext">The parser context.</param>
    /// <returns>The Spring.Objects.Factory.Config.IObjectDefinition.</returns>
    public static IObjectDefinition ParseInnerObjectDefinition(XmlElement element, ParserContext parserContext)
    {
      // parses out inner object definition for concrete implementation if defined
      var childElements = element.GetElementsByTagName("object");
      IObjectDefinition innerComponentDefinition = null;
      IConfigurableObjectDefinition inDef = null;

      if (childElements != null && childElements.Count == 1)
      {
        var objectElement = childElements[0] as XmlElement;

        // var odDelegate = parserContext.GetDelegate();
        var odHolder = parserContext.ParserHelper.ParseObjectDefinitionElement(objectElement);

        // odHolder = odDelegate.DecorateObjectDefinitionIfRequired(objectElement, odHolder);
        inDef = odHolder.ObjectDefinition as IConfigurableObjectDefinition;
        var objectName = ObjectDefinitionReaderUtils.GenerateObjectName(inDef, parserContext.Registry);

        // innerComponentDefinition = new ObjectComponentDefinition(inDef, objectName);
        parserContext.Registry.RegisterObjectDefinition(objectName, inDef);
      }

      var aRef = element.GetAttribute(REF_ATTRIBUTE);
      AssertUtils.IsTrue(
        !(!string.IsNullOrWhiteSpace(aRef) && inDef != null),
        "Ambiguous definition. Inner object " + (inDef == null ? string.Empty : inDef.ObjectTypeName) + " declaration and \"ref\" " + aRef + " are not allowed together."
        );

      return inDef;
    }
  }
}