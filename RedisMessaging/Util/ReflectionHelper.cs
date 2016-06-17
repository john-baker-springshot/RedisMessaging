using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedisMessaging.Util
{
  public static class ReflectionHelper
  {

    private const string nameSpaceRe = @"(?<ns>[a-z_][a-z\d_]+(\.[a-z_][a-z\d_]+)*)";
    private const string typeRe = @"(?<t>[a-z_][a-z\d_]+(\.[a-z_][a=z\d_]+)*)";
    private const string verRe = @"(?<v>Version=(\d+\.){3}\d+)";
    private const string cultureRe = @"(?<c>Culture=[^,]+)";
    private const string tokenRe = @"(,\s*(?<pkt>PublicKeyToken=([a-f\d]{16}|null)))?";
    private const string fullNameRe = @"(?<ns>[a-z_][a-z\d_]+\.+([a-z_][a-z\d_]+)*)";

    private static Regex _assemblyRegex = new Regex(String.Format(@"^{0},\s*{1},\s*{2},\s*{3}{4}",
      nameSpaceRe,
      typeRe,
      verRe,
      cultureRe,
      tokenRe), RegexOptions.IgnoreCase);


    private static Regex _fullnameRegex = new Regex(String.Format(@"^{0}\.{1}", nameSpaceRe, nameSpaceRe), RegexOptions.IgnoreCase);



    public static Type GetTypeByName(string className)
    {
      if (_assemblyRegex.Match(className).Success)
      {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(type => type.AssemblyQualifiedName == className)).FirstOrDefault();
        //return (from a in AppDomain.CurrentDomain.GetAssemblies()
        //        select (from aType in a.GetTypes()
        //                where aType.AssemblyQualifiedName == className
        //                select aType)
        //          .FirstOrDefault()).FirstOrDefault();
      }

      if (_fullnameRegex.Match(className).Success)
      {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(type => type.FullName == className)).FirstOrDefault();
      }

      return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(type => type.Name == className)).FirstOrDefault();

      //return (from a in AppDomain.CurrentDomain.GetAssemblies()
      //        select (from aType in a.GetTypes()
      //                where aType.Name == className
      //                select aType)
      //          .FirstOrDefault()).FirstOrDefault();
    }

  }
}
