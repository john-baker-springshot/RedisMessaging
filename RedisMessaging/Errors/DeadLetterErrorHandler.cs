using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using MessageQueue.Contracts;

namespace RedisMessaging.Errors
{
  public class DeadLetterErrorHandler : IErrorHandler
  {
    public RedisChannel Channel { get; private set; }

    public void HandleError(Exception exception, object m)
    {
      //log the exception
      var logger = LogManager.GetLogger("Error");
      logger.Error("DefaultErrorHandler reached on message "+m, exception);
      //send object to the dead letter queue
      Channel.SendToDeadLetterQueue(m.ToString());
    }
  }
}
