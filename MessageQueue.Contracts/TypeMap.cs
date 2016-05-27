using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts
{
  public class TypeMap : ITypeMap
  {
    public string Key { get; private set; }

    public Type Type { get; private set; }

    public string TypeName { get; private set;}
  }
}
