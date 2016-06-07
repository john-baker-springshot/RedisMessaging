using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts.Advices;
using RedisMessaging.Util;

namespace RedisMessaging.Errors
{
  public class ErrorAdvice: IAdvice
  {

    public string ExceptionType { get; private set; }
    public string AdviceType { get; private set; }
    public int RetryInterval { get; private set; }
    public int RetryCount { get; private set; }
    public bool RetryOnFail { get; private set; }

    private Type _exceptionType;
    private Type _adviceType;

    public Type GetExceptionType()
    {
      if (_exceptionType == null)
      {
        _exceptionType = ReflectionHelper.GetTypeByName(ExceptionType).FirstOrDefault();

        if(_exceptionType==null)// || _exceptionType!=typeof(Exception))
          throw new ArgumentException("Error registering ErrorAdvice "+ExceptionType+" is not a valid exception type");
      }
      return _exceptionType;
    }

    public Type GetAdviceType()
    {
      if (_adviceType == null)
      {
        _adviceType = ReflectionHelper.GetTypeByName(AdviceType).FirstOrDefault();

        if (AdviceType==null)// || _adviceType!=typeof(IAdvice))
          throw new ArgumentException("Error registering AdviceType " + AdviceType + " is not a valid advice type");
      }

      return _adviceType;
    }

  }
}
