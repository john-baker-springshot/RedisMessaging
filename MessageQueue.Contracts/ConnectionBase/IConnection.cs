using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.ConnectionBase
{
  public interface IConnection
  {
    /// <summary>
    /// List of all Servers this Connection is utilizing
    /// </summary>
    IList<IServer> Servers { get; set; }
    /// <summary>
    /// Returns true if at least 1 Server is connected
    /// </summary>
    bool IsConnected { get; set; }
    /// <summary>
    /// Add a Server to the List of available servers
    /// </summary>
    /// <param name="servre"></param>
    void AddServer(IServer server);
    /// <summary>
    /// Connect to the IConnection's Server(s)
    /// </summary>
    void Connect();
    /// <summary>
    /// Returns the underlying connection
    /// </summary>
    /// <returns></returns>
    object GetConnection();
  }
}
