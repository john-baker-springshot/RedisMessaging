using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisMessaging.Util;
using Spring.Validation;

namespace RedisMessaging.Tests.UtilTests
{
  [TestFixture]
  public class TestReflectionHelper
  {

    [Test]
    public void ReflectionHelper_GetStringTypeTest()
    {
      const string stringType = "ApplicationException";

      var type = ReflectionHelper.GetTypeByName(stringType);

      Assert.That(type, Is.Not.Null);
      Assert.That(type.Name, Is.EqualTo(stringType));
    }

    [Test]
    public void ReflectionHelper_GetFullNameTypeTest()
    {

      const string stringType = "System.ApplicationException";

      var type = ReflectionHelper.GetTypeByName(stringType);

      Assert.That(type, Is.Not.Null);
      Assert.That(type.FullName, Is.EqualTo(stringType));
    }

    [Test]
    public void ReflectionHelper_GetAQNameTypeTest()
    {

      const string stringType = "System.ApplicationException, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

      var type = ReflectionHelper.GetTypeByName(stringType);

      Assert.That(type, Is.Not.Null);
      Assert.That(type.AssemblyQualifiedName, Is.EqualTo(stringType));
    }
  }
}
