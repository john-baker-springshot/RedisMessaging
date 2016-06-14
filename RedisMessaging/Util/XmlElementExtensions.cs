using System;
using System.Xml;

namespace RedisMessaging.Util
{
  internal static class XmlElementExtensions
  {
    public static string ToKeyValuePairAttributeStringIfDefined(this XmlElement element, string attributeName)
    {
      return element.HasAttribute(attributeName) ? $",{attributeName}={element.GetAttribute(attributeName)}" : String.Empty;
    }

    public static bool HasAttributeForProperty(this XmlElement element, string propertyName)
    {
      return element.HasAttribute(propertyName.ToCamelCase());
    }

    public static string GetAttributeValueForProperty(this XmlElement element, string propertyName)
    {
      return element.HasAttributeForProperty(propertyName)
        ? element.GetAttribute(propertyName.ToCamelCase())
        : String.Empty;
    }

    /// <summary>Determines whether [is attribute defined] [the specified element].</summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if [is attribute defined] [the specified element]; otherwise, <c>false</c>.</returns>
    public static bool IsAttributeDefined(this XmlElement element, string attributeName)
    {
      var value = element.GetAttribute(attributeName);
      return !string.IsNullOrWhiteSpace(value);
    }
  }
}