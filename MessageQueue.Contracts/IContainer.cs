using System.Collections.Generic;

namespace MessageQueue.Contracts
{
  public interface IContainer
  {
    /// <summary>
    /// Connection of the Container
    /// </summary>
    IConnection Connection { get; }
    /// <summary>
    /// List of Channels accessible by the Container
    /// </summary>
    IEnumerable<IChannel> Channels { get; }
    /// <summary>
    /// Initializes all Connections and Channels
    /// </summary>
    void Init();
  }
}
