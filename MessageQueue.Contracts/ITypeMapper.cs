using System;
using System.Collections.Generic;
using MessageQueue.Contracts.Consumer;

namespace MessageQueue.Contracts
{
  public interface ITypeMapper
  {
    Type GetTypeForKey(string key);

    IDictionary<string, Type> Types { get; } 
  }
}