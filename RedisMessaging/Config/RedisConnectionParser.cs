using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using RedisMessaging.Util;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Config
{
  public class RedisConnectionParser : AbstractSingleObjectDefinitionParser
  {
    private static readonly string ConnectionStringAttribute = "connectionString";

    private static readonly string ConnectionStringNameAttribute = "connectionStringName";

    private static readonly string Endpoints = "endpoints";

    private static readonly string AbortOnConnectFail = "abortOnConnectFail";

    private static readonly string AllowAdmin = "allowAdmin";

    private static readonly string ChannelPrefix = "channelPrefix";

    private static readonly string ConnectRetry = "connectRetry";

    private static readonly string ConnectTimeout = "connecTimeout";

    private static readonly string ConfigChannel = "configChannel";

    private static readonly string DefaultDatabase = "defaultDatabase";

    private static readonly string KeepAlive = "keepAlive";

    private static readonly string Password = "password";

    private static readonly string Proxy = "proxy";

    private static readonly string ResolveDns = "resolveDns";

    private static readonly string Ssl = "ssl";

    private static readonly string SslHost = "sslHost";

    private static readonly string SyncTimeout = "syncTimeout";

    private static readonly string TieBreaker = "tiebreaker";

    private static readonly string Version = "version";

    private static readonly string WriteBufferSize = "writeBuffer";

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
      return typeof(RedisConnection);
    }

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
      //this should take precedence over ConnectionString
      if (element.HasAttribute(ConnectionStringNameAttribute))
      {
        if (element.Attributes.Cast<XmlAttribute>().Any(a => a.Name != ConnectionStringNameAttribute && a.Name != "id"))
        {
          parserContext.ReaderContext.ReportFatalException(element, "If the connectionStringName attribute is used, then other connection attributes should not be specified");
        }
        else
        {
          var value = element.GetAttribute(ConnectionStringNameAttribute);
          var conString = ConfigurationManager.ConnectionStrings[value].ConnectionString;
          if (!String.IsNullOrEmpty(conString))
          {
            builder.AddConstructorArg(new TypedStringValue(conString));
            return;
          }
        }
        
      }

      if (element.HasAttribute(ConnectionStringAttribute))
      {
        if (element.Attributes.Cast<XmlAttribute>().Any(a => a.Name != ConnectionStringAttribute && a.Name != "id"))
        {
          parserContext.ReaderContext.ReportFatalException(element, "If the connectionString attribute is used, then other connection attributes should not be specified");
        }
        else
        {
          NamespaceUtils.AddConstructorArgValueIfAttributeDefined(builder, element, ConnectionStringAttribute);
          return;
        }
      }

      //if connectionString attribute was not specified than servers should be specified.
      if (!element.HasAttribute(Endpoints))
      {
        parserContext.ReaderContext.ReportFatalException(element, "Either the connection string containing the endpoints or the Endpoints attribute is required.");
      }

      //to rely on the out-of-the-box ability provided by ConfiguratioOptions.Parse(connectionString) method,
      //we'll now read all of the attributes and form a connection string.

      var connectionString = new StringBuilder();
      connectionString.Append(element.GetAttribute(Endpoints));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(AbortOnConnectFail));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(AllowAdmin));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(ChannelPrefix));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(ConnectRetry));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(ConnectTimeout));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(ConfigChannel));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(DefaultDatabase));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(KeepAlive));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(Password));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(Proxy));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(ResolveDns));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(Ssl));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(SslHost));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(SyncTimeout));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(TieBreaker));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(Version));
      connectionString.Append(element.ToKeyValuePairAttributeStringIfDefined(WriteBufferSize));

      builder.AddConstructorArg(new TypedStringValue(connectionString.ToString()));
    }

    #endregion
  }
}