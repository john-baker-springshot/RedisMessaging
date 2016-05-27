using System;

namespace MessageQueue.Contracts
{
  public interface IErrorHandler
  {
    /// <summary>
    /// Handles the error.
    /// </summary>
    /// <param name="exception">The exception.</param>
    void HandleError(Exception exception, object m);
  }
}
