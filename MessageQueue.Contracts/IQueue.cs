namespace MessageQueue.Contracts
{
  public interface IQueue
  {
    /// <summary>
    /// Name of the Queue
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Time to live of the Messages within the Queue
    /// </summary>
    int TTL { get; set; }

  }
}
