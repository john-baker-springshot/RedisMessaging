using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.ConnectionBase
{
  public interface IServer
  {
    /// <summary>
    /// Name of the backend storage option
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Endpoint of the backend storage option
    /// </summary>
    string Endpoint { get; set; }
    /// <summary>
    /// Username to connect to the Enpoint
    /// </summary>
    string User { get; set; }
    /// <summary>
    /// Password to connect to the Endpoint
    /// </summary>
    string Pass { get; set; }
  }
}
