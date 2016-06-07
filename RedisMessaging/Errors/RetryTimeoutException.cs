using System;
using MessageQueue.Contracts.Advices;

namespace RedisMessaging.Errors
{
  public class RetryTimeoutException : IRetryRequeueAdvice
  {
    public bool RetryOnFail { get; private set; }

    Type IAdvice.GetType()
    {
      return typeof(TimeoutException);
    }
  }
}
