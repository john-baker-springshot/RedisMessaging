using System;

namespace MessageQueue.Contracts.Advices
{
  //TODO: Should this inherit as IAdvice<T> instead?
  public interface ITimedRetryAdvice : IAdvice<Exception>
  {
    /// <summary>
    /// Maximum number of retries allowed before auto-failure
    /// </summary>
    int RetryCount { get; }

    /// <summary>
    /// Amount of time (in ms) before the Message will attempt to be reprocessed
    /// </summary>
    int RetryInterval { get; }
  }
}
