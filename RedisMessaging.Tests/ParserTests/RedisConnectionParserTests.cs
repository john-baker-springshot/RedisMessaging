﻿using System;
using Common.Logging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RedisMessaging.Config;
using Spring.Core.IO;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Xml;

namespace RedisMessaging.Tests.ParserTests
{
  [TestFixture]
  public class RedisConnectionParserTests
  {
    private static readonly ILog Logger = LogManager.GetLogger<RedisConnectionParserTests>();

    [SetUp]
    public void Setup()
    {
      NamespaceParserRegistry.RegisterParser(typeof(RedisNamespaceHandler));
    }


    [Test]
    public void TestWithConnectionString()
    {
      var objectFactory = LoadContext(1);

      var redisConnection = objectFactory.GetObject<RedisConnection>("strongConnection");

      Assert.NotNull(redisConnection);
    }

    [Test]
    public void TestWithSpecificOptions()
    {
      const int expectedDefaultDatabase = 5;
      const int expectedConnectRetryCount = 5;
      const int expectedKeepAliveSeconds = 20;
      const int expectedSyncTimeout = 7500;
      const string expectedTieBreakerKey = "breakerKey";
      const string expectedChannelPrefix = "pfx";
      const string expectedConfigChannel = "ch1";
      const string expectedVersion = "3.0.4";
      const int expectedWriteBuffer = 100;
      const string expectedSslHost = "dev-redis.springshot.com";

      var objectFactory = LoadContext(2);

      var redisConnection = objectFactory.GetObject<RedisConnection>("strongConnection");

      Assert.IsTrue(redisConnection.Config.EndPoints.Count > 0);
      Assert.IsNotEmpty(redisConnection.Config.Password);
      Assert.AreEqual(expectedDefaultDatabase, redisConnection.Config.DefaultDatabase);
      Assert.True(redisConnection.Config.AbortOnConnectFail);
      Assert.True(redisConnection.Config.AllowAdmin);
      Assert.AreEqual(expectedChannelPrefix, redisConnection.Config.ChannelPrefix.ToString());
      Assert.AreEqual(expectedConnectRetryCount, redisConnection.Config.ConnectRetry);
      Assert.AreEqual(expectedConfigChannel, redisConnection.Config.ConfigurationChannel);
      Assert.AreEqual(expectedKeepAliveSeconds, redisConnection.Config.KeepAlive);
      Assert.True(redisConnection.Config.ResolveDns);
      Assert.False(redisConnection.Config.Ssl);
      Assert.AreEqual(expectedSslHost, redisConnection.Config.SslHost);
      Assert.AreEqual(expectedSyncTimeout, redisConnection.Config.SyncTimeout);
      Assert.AreEqual(expectedTieBreakerKey, redisConnection.Config.TieBreaker);
      Assert.AreEqual(expectedVersion, redisConnection.Config.DefaultVersion.ToString());
      Assert.AreEqual(expectedWriteBuffer, redisConnection.Config.WriteBuffer);
    }

    [Test]
    public void TestWithConnectionStringAndSpecificOptions()
    {
      Assert.Throws<ObjectDefinitionStoreException>(() =>
      {
        var objectFactory = LoadContext(3);
      });
    }

    private XmlObjectFactory LoadContext(int configId)
    {
      var resourceName = $"assembly://RedisMessaging.Tests/RedisMessaging.Tests.Configs/Connection-{configId}.config";
      Logger.Info(m => m($"Resource Name: {resourceName}"));

      var resource = new AssemblyResource(resourceName);
      return new XmlObjectFactory(resource);
    }
  }
}