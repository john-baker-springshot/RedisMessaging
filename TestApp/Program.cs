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
      int messages = 0;
      var _container = ContextRegistry.GetContext();
      var producer = _container.GetObject<IProducer>("MyProducer");
      var consumer = _container.GetObject<IContainer>("MyContainer");
      //Console.WriteLine("Listener Instances "+consumer.Channels.First().Listeners.First().Count);
      //Console.WriteLine("Enter number of messages");
      Stopwatch sw = new Stopwatch();
      sw.Start();
      const int maxMessage = 100000;
      producer.Connection.Connect();

      for (int i = 0; i < maxMessage; i++)
      {
        producer.Publish(CreateBasicMessage(i, "hey hey hey"));
      }
      sw.Stop();
      Console.WriteLine("Producer finished in: "+sw.ElapsedMilliseconds);
      //Console.ReadLine();
      sw = new Stopwatch();
      sw.Start();
      consumer.Init();
      var conn = (RedisConnection)consumer.Connection;
      while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      {
        //do nothing
      }
      //messages =  Int32.Parse("100000");
      //Stopwatch sw = new Stopwatch();
      //sw.Start();
      //for (int i = 0; i < messages; i++)
      //{
      //  producer.Publish(CreateBasicMessage(i, "hey hey hey"));
      //}
      ////sw.Stop();
      ////Console.WriteLine(sw.ElapsedMilliseconds);
      ////Console.WriteLine("Run consumer:");
      ////Console.ReadLine();
      ////GC.Collect();
      ////Console.WriteLine("GC Done");
      ////sw = new Stopwatch();
      ////sw.Start();
      ////Console.WriteLine("Init start "+sw.ElapsedMilliseconds);
      //consumer.Init();
      ////Console.WriteLine("Init done " + sw.ElapsedMilliseconds);
      //var conn = (RedisConnection)consumer.Connection;
      //while (conn.Multiplexer.GetDatabase().ListLength("MessageQueue") > 0)
      //{
      //  //do nothing
      //}
      sw.Stop();
      Console.WriteLine("Consumer finished in: "+sw.ElapsedMilliseconds);
      Console.ReadLine();
      _container.Dispose();
    }
    public static string CreateBasicMessage(int number, string message)
    {
      KeyValuePair<string, BasicMessage> kvp = new KeyValuePair<string, BasicMessage>("Basic:" + number, new BasicMessage(message));
      return JsonConvert.SerializeObject(kvp);
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
}
