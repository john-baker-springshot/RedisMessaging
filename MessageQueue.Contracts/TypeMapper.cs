using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageQueue.Contracts
{
  public class TypeMapper : ITypeMapper
  {
    private IEnumerable<ITypeMap> _typeMaps;

    #region Implementation of ITypeMapper

    public virtual IEnumerable<ITypeMap> TypeMaps => _typeMaps;

    public virtual Type GetTypeForKey(string key)
    {
      foreach (ITypeMap tmap in TypeMaps)
      {
        if (tmap.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
          return Type.GetType(tmap.TypeName);
      }
      return null;
      //var res = (from tmap in TypeMaps where tmap.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) select Type.GetType(tmap.TypeName)).FirstOrDefault();
      //return res;
    }

    #endregion

  }
}