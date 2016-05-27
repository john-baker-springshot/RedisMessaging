using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.Contracts
{
  public interface IMessageConverter
  {
    ITypeMapper TypeMapper { get; }
    bool CreateMessageIds { get; }
    object Convert(string message, out string key);
  }
}
