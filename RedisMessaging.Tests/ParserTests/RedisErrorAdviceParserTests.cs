using System;
using System.Data.SqlClient;
using MessageQueue.Contracts;
using NUnit.Framework;
using RedisMessaging.Config;
using RedisMessaging.Errors;
using RedisMessaging.Tests.UtilTests;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisErrorAdviceParserTests
  {
    private readonly string ConfigConventionPrefix = "ErrorAdvice";

    [OneTimeSetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }

    [Test]
    public void TestTimedRetryAdvice()
    {
      const bool expectedRetryOnFail = true;
      const AdviceType expectedAdviceType = AdviceType.TimedRetry;
      const string expectedExceptionType = "TimeoutException";
      var expectedTimeoutException = typeof(TimeoutException);
      const int expectedRetryCount = 8;
      const int expectedRetryInterval = 30;

      var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, 1);

      var timedErrorAdvice = objectFactory.GetObject<ErrorAdvice>("advice1");

      Assert.NotNull(timedErrorAdvice);
      Assert.AreEqual(expectedRetryOnFail, timedErrorAdvice.RetryOnFail);
      Assert.AreEqual(expectedAdviceType, timedErrorAdvice.AdviceType);
      Assert.AreEqual(expectedExceptionType, timedErrorAdvice.ExceptionType);
      Assert.AreEqual(expectedTimeoutException, timedErrorAdvice.GetExceptionType());
      Assert.AreEqual(expectedRetryCount, timedErrorAdvice.RetryCount);
      Assert.AreEqual(expectedRetryInterval, timedErrorAdvice.RetryInterval);
    }

    [Test]
    public void TestRetryRequeueAdvice()
    {
      const bool expectedRetryOnFail = true;
      const AdviceType expectedAdviceType = AdviceType.RetryRequeue;
      const string expectedExceptionType = "System.Data.SqlClient.SqlException";
      var expectedTimeoutException = typeof(SqlException);

      var objectFactory = ParserTestsHelper.LoadContext(ConfigConventionPrefix, 1);

      var timedErrorAdvice = objectFactory.GetObject<ErrorAdvice>("advice2");

      Assert.NotNull(timedErrorAdvice);
      Assert.AreEqual(expectedRetryOnFail, timedErrorAdvice.RetryOnFail);
      Assert.AreEqual(expectedAdviceType, timedErrorAdvice.AdviceType);
      Assert.AreEqual(expectedExceptionType, timedErrorAdvice.ExceptionType);
      Assert.AreEqual(expectedTimeoutException, timedErrorAdvice.GetExceptionType());
    }

    [Test]
    public void TestInvalidRetryRequeueAdviceWithRetryCount()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfigConventionPrefix, 2);
      });
    }

    [Test]
    public void TestInvalidRetryRequeueAdviceWithRetryInterval()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        ParserTestsHelper.LoadContext(ConfigConventionPrefix, 3);
      });
    }
  }
}