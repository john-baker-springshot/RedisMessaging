using System;

namespace MessageQueue.Contracts
{
  public interface ITypeMap
  {
    /// <summary>
    /// String Key representing the Type of the TypeMap
    /// </summary>
    string Key { get; set; }
    /// <summary>
    /// Type of the TypeMap
    /// </summary>
    Type Type { get; set; }
  }
}
