using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts
{
  public enum AdviceType
  {
    RetryRequeue,
    TimedRetry
  }
}
