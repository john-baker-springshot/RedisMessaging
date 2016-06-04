using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Common.Logging;
using MessageQueue.Contracts;
using StackExchange.Redis;

namespace RedisMessaging
{
  public class RedisConnection : IConnection
  {
    private readonly string _connectionString;

    private static Lazy<ConnectionMultiplexer> _lazyConnection;
    private readonly ConfigurationOptions _config;
    private static readonly ILog Log = LogManager.GetLogger(typeof(RedisConnection));
    public RedisConnection(string connectionString) : this(connectionString, null) { }

    public RedisConnection(string connectionString, string pass)
    {
      _connectionString = connectionString;

      _config = new ConfigurationOptions
      {
        EndPoints = { connectionString},
      };

      if (pass != null)
      {
        Pass = pass;
        _config.Password = pass;
      }
      
      _lazyConnection = new Lazy<ConnectionMultiplexer>(
      () =>
      {
        return ConnectionMultiplexer.Connect(_config);
      });
      Connect();
      Log.Info("Redis Connection connected to "+connectionString);
    }

    //need a way to turn this on/off
    /// <summary>
    /// Endpoint of the backend storage option
    /// </summary>
    public virtual string Endpoint { get; private set; }

    /// <summary>
    /// Username to connect to the Enpoint
    /// </summary>
    public virtual string User
    {
      get
      {
        throw new NotSupportedException("Redis doesn't use usernames for authentication.");
      }
    }

    /// <summary>
    /// Password to connect to the Endpoint
    /// </summary>
    public virtual string Pass { get; private set; }

    public bool IsConnected { get; private set; }

    internal IConnectionMultiplexer Multiplexer { get { return _lazyConnection.Value; } }

    public void Connect()
    {
      if (IsConnected)
        return;

      if (Multiplexer.IsConnected)
      {
        IsConnected = true;
      }
      else
      {
        throw new Exception("Database not reachable");
      }     
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      Multiplexer.Dispose();
      IsConnected = false;
    }
  }
}
