using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQSauce
{
    public class Message
    {
        public Guid ID { get; set; }
        public object Payload { get; set; }

        public Message()
        {
            ID = Guid.NewGuid();
        }
    }

    public class Message<T> : Message
    {
        public T Value
        {
            get
            {
                var str = JsonConvert.SerializeObject(Payload);
                return JsonConvert.DeserializeObject<T>(str);                
            }
            set
            {
                Payload = value;
            }
        }
    }
}
