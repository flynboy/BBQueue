using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQSauce.Test
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
}
