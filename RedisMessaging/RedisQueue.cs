using MessageQueue.Contracts;

namespace RedisMessaging
{
  public class RedisQueue : IQueue
  {
    public string Name { get; set; }

    public int TTL { get; set; }

    public RedisQueue(string name, int ttl)
    {
      Name = name;
      TTL = ttl;
    }
  }
}
