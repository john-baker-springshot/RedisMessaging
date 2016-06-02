using System;
using MessageQueue.Contracts.Advices;

namespace RedisMessaging.Errors
{
  public class RetryTimeoutException : IRetryRequeueAdvice<TimeoutException>
  {
    public bool RetryOnFail { get; private set; }

    Type IAdvice<TimeoutException>.GetType()
    {
      return typeof(TimeoutException);
    }
  }
}
