using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Model
{
    public enum AccountStatus
    {
        Active,
        InActive
    }

    public class Account
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Secret { get; set; }

        public Account()
        {
            ID = Guid.NewGuid();
        }
    }
}
