using MessageQueue.Contracts.Consumer;
using System;

namespace RedisMessaging.Consumer
{
  public class RedisTypeMapper : ITypeMap
  {
    public string Key { get; set; }

    public Type Type { get; set; }
  }
}
