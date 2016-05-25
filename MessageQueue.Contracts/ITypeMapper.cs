using System;
using System.Collections.Generic;
using MessageQueue.Contracts.Consumer;

namespace MessageQueue.Contracts
{
  public interface ITypeMapper
  {
    IEnumerable<ITypeMap> TypeMaps { get; }

    Type GetTypeForKey(string key);
  }
}