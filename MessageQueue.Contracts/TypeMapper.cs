using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageQueue.Contracts
{
  public class TypeMapper : ITypeMapper
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Object"/> class.
    /// </summary>
    public TypeMapper()
    {
      Types = new Dictionary<string, Type>();
    }

    #region Implementation of ITypeMapper

    public virtual Type GetTypeForKey(string key)
    {
      return Types.Where(type => type.Key.Equals(key)).Select(type => type.Value).FirstOrDefault();
    }

    public IDictionary<string, Type> Types { get; }

    #endregion

  }
}