using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts.Consumer
{
  public interface ITypeMap
  {
    /// <summary>
    /// String Key representing the Type of the TypeMap
    /// </summary>
    string Key { get; set; }
    /// <summary>
    /// Type of the TypeMap
    /// </summary>
    Type Type { get; set; }
  }
}
