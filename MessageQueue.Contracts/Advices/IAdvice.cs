using System;

namespace MessageQueue.Contracts.Advices
{
  public interface IAdvice<out T> where T : Exception
  {
    /// <summary>
    /// Boolean flag for retrying message execution on a specific exception occurence
    /// </summary>
    bool RetryOnFail { get; set; }
    /// <summary>
    /// Returns the Type of the IAdvice Exception
    /// </summary>
    /// <returns></returns>
    T GetType();
  }
}
