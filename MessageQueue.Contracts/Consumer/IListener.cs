using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Consumer
{
  public interface IListener
  {
    /// <summary>
    /// Number of instances that should be spawned of this Listener type
    /// </summary>
    int Count { get; set; }
    /// <summary>
    /// MessageHandlers designated for this Listener
    /// </summary>
    IMessageHandler MessageHandler { get; set; }
    /// <summary>
    /// TypeKey representing the Type of messages this Listener should handle
    /// </summary>
    Type TypeKey { get; set; }
    /// <summary>
    /// References Container this Listener is part of
    /// </summary>
    IContainer Container { get; set; }
    /// <summary>
    /// Async process that passes the message to the appropriate MessageHandler
    /// </summary>
    /// <param name="m">Message</param>
    void InternalHander(object m);
  }
}
