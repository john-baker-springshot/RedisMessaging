using MessageQueue.Contracts;

namespace RedisMessaging
{
  public class RedisQueue : IQueue
  {
    public string Name { get; }

    public int TTL { get; }

    public RedisQueue(string name, int ttl)
    {
      Name = name;
      TTL = ttl;
    }
  }
}
