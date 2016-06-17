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

namespace TestApp
{
  class Program
  {
    public static void Main(string[] args)
    {
      var container = ContextRegistry.GetContext();
      var producer = container.GetObject<IProducer>("MyProducer");
      bool test = true;
      while (test)
      {
        Console.WriteLine("Enter number of messages");
        int num = Int32.Parse(Console.ReadLine());
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Produce(num, producer);
        sw.Stop();
        Console.WriteLine("Producer finished in: " + sw.ElapsedMilliseconds);
        Console.WriteLine("Continue?");
        var y = Console.ReadLine();
        if (y.Equals("n"))
          test = false;
      }
      container.Dispose();
    }

    public static string CreateBasicMessage(int number, string message)
    {
      KeyValuePair<string, BasicMessage> kvp = new KeyValuePair<string, BasicMessage>("Basic:" + number, new BasicMessage(message));
      return JsonConvert.SerializeObject(kvp);
    }

    public static string CreateSpecificMessage(int number)
    {
      Event e = new Event
      {
        EventCategory = 0,
        EventType = 0,
        EntityId = 0,
        IsHandled = false
      };
      KeyValuePair<string, Event> kvp = new KeyValuePair<string, Event>("Event:" + number, e);
      return JsonConvert.SerializeObject(kvp);
    }

    public static void Produce(int number, IProducer producer)
    {
      producer.Connection.Connect();

      var redisConnection = (RedisConnection)producer.Connection;

      Console.WriteLine($"Publishing messages to {redisConnection.Config.SslHost}/{redisConnection.Config.DefaultDatabase}");

      for (int i = 0; i < number; i++)
      {
        //producer.Publish(CreateBasicMessage(i, "hey hey hey"));
        producer.Publish(CreateSpecificMessage(i));
      }
    }

    public static void Consume(IContainer consumer)
    {
      
      var conn = (RedisConnection)consumer.Connection;
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      {
        //do nothing
      }
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
