using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;
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
    private AdviceType _adviceType;

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

    public AdviceType GetAdviceType()
    {
      var test = (AdviceType)Enum.Parse(typeof(AdviceType), AdviceType);
      return test;
    }

  }
}
