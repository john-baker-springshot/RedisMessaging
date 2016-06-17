using System;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Advices;
using RedisMessaging.Util;

namespace RedisMessaging.Errors
{
  public class ErrorAdvice: IAdvice
  {
    public AdviceType AdviceType { get; private set; }

    public string ExceptionType { get; private set; }

    public int RetryInterval { get; private set; }

    public int RetryCount { get; private set; }

    public bool RetryOnFail { get; private set; }

    private Type _exceptionType;
    //private AdviceType _adviceType;

    public Type GetExceptionType()
    {
      if (_exceptionType == null)
      {
        _exceptionType = ReflectionHelper.GetTypeByName(ExceptionType);

        if(_exceptionType==null)// || _exceptionType!=typeof(Exception))
          throw new ArgumentException("Error registering ErrorAdvice "+ExceptionType+" is not a valid exception type");
      }
      return _exceptionType;
    }
  }
}
