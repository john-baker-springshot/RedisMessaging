using MessageQueue.Contracts.ConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Consumer
{
  public interface IContainer
  {
    /// <summary>
    /// Connection of the Container
    /// </summary>
    IConnection Connection { get; set; }
    /// <summary>
    /// List of Channels accessible by the Container
    /// </summary>
    IList<IChannel> Channels { get; set; }
    /// <summary>
    /// Initializes all Connections and Channels
    /// </summary>
    void Init();
  }
}
