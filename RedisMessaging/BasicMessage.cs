using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisMessaging
{
  public class BasicMessage
  {

    public string Message { get; set; }

    public BasicMessage(string message)
    {
      Message = message;
    }

  }
}
