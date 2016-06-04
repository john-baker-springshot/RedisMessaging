using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueue.Contracts;
using MessageQueue.Contracts.Producer;
using RedisMessaging;
using RedisMessaging.Producer;
using Spring.Context;
using Spring.Context.Support;
using Newtonsoft.Json;
using Spring.Globalization.Formatters;

namespace Consumer
{
  class Program
  {
    public static void Main(string[] args)
    {
      var container = ContextRegistry.GetContext();
      var consumer = container.GetObject<IContainer>("MyContainer");
      consumer.Init();
      Consume(consumer);
    }

  
    public static void Consume(IContainer consumer)
    {
      var conn = (RedisConnection)consumer.Connection;
      Stopwatch sw = new Stopwatch();
      
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") == 0)
      {
        //wait
      }
      sw.Start();
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      {
        //wait again
      }
      sw.Stop();
      Console.WriteLine(sw.ElapsedMilliseconds);
      Console.WriteLine("Continue?");
      var y = Console.ReadLine();
      if(y.Equals("y"))
        Consume(consumer);
    }
  }

  public class TestMessageHandler
  {
    public void HandleMessage(BasicMessage m)
    {
      Task.Delay(5000);
      //Console.WriteLine(m.Message);
    }
  }

  public class Event
  {
    public virtual int EventCategory { get; set; }
    public virtual int EventType { get; set; }
    public virtual int EntityId { get; set; }
    public virtual bool IsHandled { get; set; }
    public virtual int FailedCount { get; set; }
    //public virtual DateTime? DateCreated { get; set; }
    public virtual int Status { get; set; }
    //public virtual DateTime? DateScheduled { get; set; }
    public virtual string Tag { get; set; }
    public virtual string Data { get; set; }
    public virtual long Id { get; set; }
    public virtual string CreatedBy { get; set; }
    //public virtual DateTime? CreatedOn { get; set; }
    public virtual string ModifiedBy { get; set; }
    //public virtual DateTime? ModifiedOn { get; set; }
    public virtual bool IsTransient()
    {
      return Id <= 0;
    }
  }

}
