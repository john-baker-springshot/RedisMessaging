using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

    public static bool HasChildElement(this XmlElement element, string childElementName)
    {
      //var hasChildElement = element.ChildNodes.Cast<XmlNode>()
      //  .Any(node => node.NodeType == XmlNodeType.Element && node.LocalName == childElementName);

      foreach (var childNode in element.ChildNodes.Cast<XmlNode>())
      {
        if ((childNode.NodeType == XmlNodeType.Element && childNode.LocalName == childElementName) || HasChildElement((XmlElement)childNode, childElementName))
        {
          return true;
        }
      }

      return false;
    }

    public static XmlElement GetSingleChildElement(this XmlElement parentElement, string childElementName)
    {
      //return (XmlElement)parentElement.ChildNodes.Cast<XmlNode>()
      //  .SingleOrDefault(node => node.NodeType == XmlNodeType.Element && node.LocalName == childElementName);

      foreach (var childNode in parentElement.ChildNodes.Cast<XmlNode>())
      {
        if ((childNode.NodeType == XmlNodeType.Element && childNode.LocalName == childElementName))
        {
          return (XmlElement) childNode;
        }

        var childElement = GetSingleChildElement((XmlElement) childNode, childElementName);
        if (childElement != null)
          return childElement;
      }

      return null;
    }
  }
}