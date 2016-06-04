using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using MessageQueue.Contracts;
using Newtonsoft.Json;

namespace RedisMessaging
{
  public class JsonMessageConverter : IMessageConverter
  {
    public ITypeMapper TypeMapper { get; private set; }

    public bool CreateMessageIds { get; private set; }

    private static readonly ILog Log = LogManager.GetLogger(typeof(JsonMessageConverter));

    public object Convert(string message, out string key)
    {
      //pull the key out of the message
      var kvpMessage = JsonConvert.DeserializeObject<KeyValuePair<string, object>>(message);
      key = kvpMessage.Key.Split(':')[0];
      //get the type of the underlying message object by key
      var messageType = TypeMapper.GetTypeForKey(key);
      if (messageType == null)
        return null;
      //deserialize and return the message object as the concrete type
      var concreteMessage = JsonConvert.DeserializeObject(kvpMessage.Value.ToString(), messageType);
      Log.Debug("Message "+message+" converted to type "+messageType);
      return concreteMessage;
    }
  }
}
