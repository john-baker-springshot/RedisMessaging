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

namespace TestApp
{
  class Program
  {
    public static void Main(string[] args)
    {
      var container = ContextRegistry.GetContext();
      var producer = container.GetObject<IProducer>("MyProducer");
      var consumer = container.GetObject<IContainer>("MyContainer");
      //Console.WriteLine("Listener Instances "+consumer.Channels.First().Listeners.First().Count);
      //Console.WriteLine("Enter number of messages");
      Stopwatch sw = new Stopwatch();
      sw.Start();
      Produce(10, producer);
      sw.Stop();
      Console.WriteLine("Producer finished in: "+sw.ElapsedMilliseconds);
      //Console.ReadLine();
      sw = new Stopwatch();
      sw.Start();
      //Consume(consumer);
      sw.Stop();
      Console.WriteLine("Consumer finished in: "+sw.ElapsedMilliseconds);
      Console.ReadLine();
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

      for (int i = 0; i < number; i++)
      {
        //producer.Publish(CreateBasicMessage(i, "hey hey hey"));
        producer.Publish(CreateSpecificMessage(i));
      }
    }

    public static void Consume(IContainer consumer)
    {
      consumer.Init();
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
    public virtual DateTime? DateCreated { get; set; }
    public virtual int Status { get; set; }
    public virtual DateTime? DateScheduled { get; set; }
    public virtual string Tag { get; set; }
    public virtual string Data { get; set; }
    public virtual long Id { get; set; }
    public virtual string CreatedBy { get; set; }
    public virtual DateTime? CreatedOn { get; set; }
    public virtual string ModifiedBy { get; set; }
    public virtual DateTime? ModifiedOn { get; set; }
    public virtual bool IsTransient()
    {
      return Id <= 0;
    }
  }

}
