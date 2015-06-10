using BBQ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Memory
{
    public class QueueRepository : IQueueRepository
    {
        protected static List<Queue> Queues = new List<Queue>();

        protected Guid AccountID { get; set; }

        public bool Add(Model.Queue a)
        {
            if (a == null || a.AccountID==Guid.Empty) return false;

            Queues.Add(a);
            return true;
        }

        public bool Delete(Guid id)
        {
            var q = Get(id);
            if (q == null) return false;
            Queues.Remove(q);
            return true;
        }

        public Model.Queue Get(Guid id)
        {
            return Queues.SingleOrDefault(q => q.AccountID == AccountID && q.ID == id);
        }

        public bool Save(Model.Queue a)
        {
            var q = Get(a.ID);
            if (q == null) return false;

            Queues.Remove(q);
            Queues.Add(q);

            return true;
        }

        public Queue GetByName(string Name)
        {
            return Queues.SingleOrDefault(q => q.AccountID == AccountID && q.Name == Name);
        }

        public IEnumerable<Queue> GetAll()
        {
            return Queues.Where(q => q.AccountID == AccountID);
        }

        public bool Init(Guid AccountID, string ConnectionString="")
        {
            this.AccountID = AccountID;
            return true;
        }


        public decimal Count
        {
            get
            {
                return Queues.Where(q => q.AccountID == AccountID).Count();
            }
        }
    }
}
