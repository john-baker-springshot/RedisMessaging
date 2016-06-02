using System;

namespace MessageQueue.Contracts.Advices
{
  //TODO: Should this inherit as IAdvice<T> instead?
  public interface IRetryRequeueAdvice<out T> : IAdvice<T> where T:Exception
  {
    //no required implementation
  }
}
