using System;

namespace MessageQueue.Contracts.Advices
{
  public interface IAdvice
  {
    /// <summary>
    /// Boolean flag for retrying message execution on a specific exception occurence
    /// </summary>
    bool RetryOnFail { get; }
    /// <summary>
    /// Returns the Type of the IAdvice Exception
    /// </summary>
    /// <returns></returns>
    Type GetType();
  }
}
