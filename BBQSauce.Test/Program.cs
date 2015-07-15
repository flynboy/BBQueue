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
        public class TestMessage
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public List<string> Roles { get; set; }

            public TestMessage()
            {
                ID = Guid.NewGuid();
                Roles = new List<string>();
            }
        }

        static void Main(string[] args)
        {
            const int repeat_count = 100;

            Stopwatch t = new Stopwatch();
            Stopwatch add = new Stopwatch();
            Stopwatch get = new Stopwatch();
            Stopwatch commit = new Stopwatch();

            t.Start();

            BBQ.BaseUri = @"https://services/Queue/v1/";
            BBQ.AccountID = Guid.NewGuid().ToString();
            BBQ.Secret = DateTime.Now.Ticks.ToString();

            var token = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", BBQ.AccountID, BBQ.Secret)));
            Console.WriteLine(token);

            //get all queues
            var bbq = new BBQ();
            var q = bbq.GetOrCreateQueue("testq");
            
            //check stats
            var stats = bbq.GetStats();
            displayStats(stats);

            stats = bbq.GetStats(q);
            displayStats(stats, q);

            for (var i = 0; i < repeat_count; i++)
            {
                add.Start();
                var result = bbq.Enqueue(q, new TestMessage()
                    {
                        Name = "test1",
                        Roles = new List<string>() { "one", "two" }
                    });
                add.Stop();
            }

            stats = bbq.GetStats();
            displayStats(stats);

            stats = bbq.GetStats(q);
            displayStats(stats, q);

            for (var i = 0; i < repeat_count; i++)
            {
                get.Start();
                var msg = bbq.DeQueue<TestMessage>(q);
                get.Stop();
                commit.Start();
                var removed = bbq.MessageProcessed(q, msg);
                commit.Stop();
            }

            stats = bbq.GetStats();
            displayStats(stats);

            stats = bbq.GetStats(q);
            displayStats(stats, q);

            t.Stop();

            Console.WriteLine("Completed in {0} ms", t.ElapsedMilliseconds);
            Console.WriteLine("Adds {0} ms, Avg {1} ms", add.ElapsedMilliseconds, add.ElapsedMilliseconds / repeat_count);
            Console.WriteLine("Gets {0} ms, Avg {1} ms", get.ElapsedMilliseconds, get.ElapsedMilliseconds / repeat_count);
            Console.WriteLine("Commits {0} ms, Avg {1} ms", commit.ElapsedMilliseconds, commit.ElapsedMilliseconds / repeat_count);

            Console.ReadKey();
        }

        private static void displayStats(IEnumerable<Statistic> stats, Queue q=null)
        {            
            Console.WriteLine("{0}Stats:",q!=null ? q.Name +" " : "");
            Console.WriteLine("".PadRight(60, '-'));
            foreach(var s in stats)
            {
                Console.WriteLine("{0} : {1}", s.Name, s.Value);
            }
            Console.WriteLine("".PadRight(60, '-'));
        }
    }
}
