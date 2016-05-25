namespace MessageQueue.Contracts.Producer
{
  public interface IProducer
  {
    /// <summary>
    /// Publish a string Message to IQueue Queue
    /// </summary>
    /// <param name="queue">IQueue</param>
    /// <param name="message">String</param>
    void Publish(IQueue queue, string message);
    /// <summary>
    /// Publish a string Message to string Queue
    /// </summary>
    /// <param name="queue">String</param>
    /// <param name="message">String</param>
    void Publish(string queue, string message);
    /// <summary>
    /// Publish a string Message to its ascribed MessageQueue
    /// </summary>
    /// <param name="message"></param>
    void Publish(string message);
    /// <summary>
    /// Connection of the Producer
    /// </summary>
    IConnection Connection { get; }
    /// <summary>
    /// Default MessageQueue used by the Producer
    /// </summary>
    IQueue MessageQueue { get; }
  }
}
