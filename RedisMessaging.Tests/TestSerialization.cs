using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace RedisMessaging.Tests
{
  [TestFixture]
  public class TestSerialization
  {

    [Test]
    public void TwoTimesSerializationTest()
    {
      const int messages = 100000;
      Stopwatch sw = new Stopwatch();
      sw.Start();
      for (int i = 0; i < messages; i++)
      {
        var message = RedisMessagingImplementationTests.CreateBasicMessage(i, "test");
        var key = message.Substring(message.IndexOf("\"Key\":") + 5, message.IndexOf("\"Value\":") - 2);
        var kvpMessage = JsonConvert.DeserializeObject<KeyValuePair<string, object>>(message);
        var concreteMessage = JsonConvert.DeserializeObject<BasicMessage>(kvpMessage.Value.ToString());
      }
      sw.Stop();
      TestContext.WriteLine("ddsdsdsfdsfdsfdsfsdfs " + sw.ElapsedMilliseconds);
    }

    [Test]
    public void OneTimesSerializationTest()
    {
      //Debug.AutoFlush=true;
      const int messages = 100000;
      Stopwatch sw = new Stopwatch();
      sw.Start();
      for (int i = 0; i < messages; i++)
      {
        var message = RedisMessagingImplementationTests.CreateBasicMessage(i, "test");
        var kvpMessage = JsonConvert.DeserializeObject<KeyValuePair<string, object>>(message);
        //var concreteMessage = JsonConvert.DeserializeObject<BasicMessage>(kvpMessage.Value.ToString());
      }
      sw.Stop();
      TestContext.WriteLine("ddsdsdsfdsfdsfdsfsdfs "+ sw.ElapsedMilliseconds);
    }

  }
}
