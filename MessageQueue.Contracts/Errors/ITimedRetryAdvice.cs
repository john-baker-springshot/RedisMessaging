using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Errors
{
  public interface ITimedRetryAdvice : IAdvice<Exception>
  {
    /// <summary>
    /// Maximum number of retries allowed before auto-failure
    /// </summary>
    int RetryCount { get; set; }
    /// <summary>
    /// Amount of time (in ms) before the Message will attempt to be reprocessed
    /// </summary>
    int RetryInterval { get; set; }
  }
}
