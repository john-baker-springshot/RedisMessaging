using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Consumer
{
  public interface IMessageHandler
  {
    /// <summary>
    /// Processes the Message object
    /// </summary>
    /// <param name="m"></param>
    void HandleMessage(object m);
  }
}
