using System.Collections.Generic;

namespace MessageQueue.Contracts
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
