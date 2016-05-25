using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Errors
{
  public interface IAdvice<T> where T : Exception
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
