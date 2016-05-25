using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageQueue.Contracts
{
  public class TypeMapper : ITypeMapper
  {
    private readonly IList<ITypeMap> _typeMaps;

    public TypeMapper()
    {
      _typeMaps = new List<ITypeMap>();
    }

    #region Implementation of ITypeMapper

    public virtual IEnumerable<ITypeMap> TypeMaps => _typeMaps;

    public virtual Type GetTypeForKey(string key)
    {
      return (from tmap in TypeMaps where tmap.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) select Type.GetType(tmap.TypeName)).FirstOrDefault();
    }

    #endregion

  }
}