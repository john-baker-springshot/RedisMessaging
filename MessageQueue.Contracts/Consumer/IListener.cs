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
    /// Concrete type of class that will handle messages recieved by this listener
    /// </summary>
    Type HandlerType { get; }
    /// <summary>
    /// String method name of the concrete HandlerType used to handle the message
    /// </summary>
    string HandlerMethod { get; }
    /// <summary>
    /// TypeKey representing the Type of messages this Listener should handle
    /// </summary>
    string TypeKey { get; }
    /// <summary>
    /// References Container this Listener is part of
    /// </summary>
    IChannel Channel { get; }

  }
}
