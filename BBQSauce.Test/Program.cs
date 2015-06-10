using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BBQSauce;
using System.Diagnostics;

namespace BBQSauce.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const int repeat_count = 100;

            Stopwatch t = new Stopwatch();
            t.Start();

            BBQ.BaseUri = @"http://localhost:58397/";
            BBQ.AccountID = Guid.NewGuid().ToString();
            BBQ.Secret = DateTime.Now.Ticks.ToString();

            var token = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", BBQ.AccountID, BBQ.Secret)));
            Console.WriteLine(token);

            //get all queues
            var bbq = new BBQ();

            var q = bbq.GetOrCreateQueue("testq");

            for (var i = 0; i < repeat_count; i++)
            {
                var result = bbq.Enqueue(q, new TestMessage()
                    {
                        Name = "test1",
                        Roles = new List<string>() { "one", "two" }
                    });
            }

            for (var i = 0; i < repeat_count; i++)
            {
                var msg = bbq.DeQueue<TestMessage>(q);
                var removed = bbq.MessageProcessed(q, msg);
            }

            t.Stop();

            Console.WriteLine("Completed in {0} ms", t.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
