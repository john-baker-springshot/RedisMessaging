using MessageQueue.Contracts.Advices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts
{
  public class TimeoutAdviceHandler : ITimedRetryAdvice
  {
    public int RetryCount { get; private set; }

    public int RetryInterval { get; private set; }

    public bool RetryOnFail { get; private set; }

    Exception IAdvice<Exception>.GetType()
    {
      return new TimeoutException();
    }

    public TimeoutAdviceHandler()
    {
      RetryCount = 2;
      RetryInterval = 5;
      RetryOnFail = true;
    }

  }
}
