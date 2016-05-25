using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging.Util
{
  public class ServiceLocator
  {
    private static readonly IApplicationContext Context;

    #region Constructor
    static ServiceLocator()
    {
      Context = ContextRegistry.GetContext();
    }
    #endregion

    #region Get Service
    public static T GetService<T>()
    {
      return GetService<T>(typeof(T).Name);
    }

    public static T GetService<T>(string typeName)
    {
      if (Context.GetObjectDefinitionNames().Any(v => v.Equals(typeName)) == false)
      {
        throw new ArgumentException(typeName, "Object Name not found in Spring.Net mapping files!");
      }

      return (T)Context.GetObject(typeName);
    }

    public static T GetService<T>(params object[] arguments)
    {
      return GetService<T>(typeof(T).Name, arguments);
    }

    public static T GetService<T>(string typeName, object[] arguments)
    {
      if (Context.GetObjectDefinitionNames().Any(v => v.Equals(typeName)) == false)
      {
        throw new ArgumentException("Object Name not found in Spring.Net mapping files!");
      }

      return (T)Context.GetObject(typeName, arguments);
    }

    #endregion
  }
}
