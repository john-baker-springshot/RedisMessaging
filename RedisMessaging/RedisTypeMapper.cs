using System;
using System.Collections.Generic;
using System.Linq;
using MessageQueue.Contracts;

namespace RedisMessaging
{
  public class RedisTypeMapper : ITypeMapper
  {
    private readonly IList<ITypeMap> _typeMaps;

    public RedisTypeMapper()
    {
      _typeMaps = new List<ITypeMap>();
    } 

    #region Implementation of ITypeMapper

    public virtual IEnumerable<ITypeMap> TypeMaps => _typeMaps;

    public void AddTypeMap(ITypeMap typeMap)
    {
      _typeMaps.Add(typeMap);
    }

    public virtual Type GetTypeForKey(string key)
    {
      return (from tmap in TypeMaps where tmap.Key.ToLower().Equals(key.ToLower()) select tmap.Type).FirstOrDefault();
    }

    #endregion
  }
}
