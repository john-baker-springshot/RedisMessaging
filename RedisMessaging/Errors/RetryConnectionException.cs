using System;
using MessageQueue.Contracts.Advices;
using StackExchange.Redis;

namespace RedisMessaging.Errors
{
  public class RetryConnectionException : ITimedRetryAdvice<RedisConnectionException>
  {
    public int RetryCount { get; private set; }

    public int RetryInterval { get; private set; }

    public bool RetryOnFail { get; private set; }

    Type IAdvice<RedisConnectionException>.GetType()
    {
      return typeof (RedisConnectionException);
    }

  }
}
