using System;

namespace RedisMessaging.Config
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public class NamespaceParser : Attribute
  {
    private string ns;
    private string schemaLocation;
    private Type schemaLocationAssemblyHint;

    /// <summary>
    /// Creates a new instance of <see cref="NamespaceParserAttribute"/>.
    /// </summary>
    public NamespaceParser()
    { }

    /// <summary>
    /// Gets or sets the default namespace for the configuration parser.
    /// </summary>
    /// <value>
    /// The default namespace for the configuration parser.
    /// </value>
    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    /// <summary>
    /// Gets or sets the default schema location for the configuration parser.
    /// </summary>
    /// <value>
    /// The default schema location for the configuration parser.
    /// </value>
    /// <remarks>
    /// If the <see cref="SchemaLocationAssemblyHint"/>  property is set, the <see cref="SchemaLocation"/> will always resolve to an assembly-resource 
    /// and the set <see cref="SchemaLocation"/> will be interpreted relative to this assembly.
    /// </remarks>
    public string SchemaLocation
    {
      get { return schemaLocation; }
      set { schemaLocation = value; }
    }

    /// <summary>
    /// Gets or sets a type from the assembly containing the schema
    /// </summary>
    /// <remarks>
    /// If this property is set, the <see cref="SchemaLocation"/> will always resolve to an assembly-resource 
    /// and the <see cref="SchemaLocation"/> will be interpreted relative to this assembly.
    /// </remarks>
    public Type SchemaLocationAssemblyHint
    {
      get { return schemaLocationAssemblyHint; }
      set { schemaLocationAssemblyHint = value; }
    }
  }
}
