using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BBQSauce;

namespace BBQSauce.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            BBQ.BaseUri = @"http://localhost:58397/";
            BBQ.AccountID = Guid.NewGuid().ToString();
            BBQ.Secret = DateTime.Now.Ticks.ToString();

            //get all queues
            var bbq = new BBQ();

            var q = bbq.GetOrCreateQueue("testq");

            var result = bbq.Enqueue(q, new TestMessage()
                {
                    Name = "test1",
                    Roles = new List<string>() { "one", "two" }
                });
            
            var msg = bbq.DeQueue<TestMessage>(q);
            var msg2 = bbq.DeQueue<TestMessage>(q);

            var removed = bbq.MessageProcessed(q, msg);
        }
    }
}
