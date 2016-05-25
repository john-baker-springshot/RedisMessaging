using System;

namespace MessageQueue.Contracts.Advices
{
  //TODO: Should this inherit as IAdvice<T> instead?
  public interface IRetryRequeueAdvice : IAdvice<Exception>
  {
    //no required implementation
  }
}
