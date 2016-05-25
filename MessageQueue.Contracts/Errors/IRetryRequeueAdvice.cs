using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Errors
{
  public interface IRetryRequeueAdvice : IAdvice<Exception>
  {
    //no required implementation
  }
}
