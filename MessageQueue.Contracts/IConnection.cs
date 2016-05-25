namespace MessageQueue.Contracts
{
  public interface IConnection
  {
    /// <summary>
    /// Endpoint of the backend storage option
    /// </summary>
    string Endpoint { get; }
    
    /// <summary>
    /// Username to connect to the Enpoint
    /// </summary>
    string User { get; }

    /// <summary>
    /// Password to connect to the Endpoint
    /// </summary>
    string Pass { get; }

    /// <summary>
    /// Returns true if at least 1 Server is connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connect to the IConnection's Server(s)
    /// </summary>
    void Connect();
  }
}
