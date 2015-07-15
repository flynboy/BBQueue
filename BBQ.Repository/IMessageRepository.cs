using BBQ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository
{
    public interface IMessageRepository : IRepository<Message, Guid>
    {
        Message GetNextAndLock(Guid QueueID);
        decimal Count(Guid? guid= null, MessageStatus? status = null);
        decimal AverageAge(Guid? guid = null, MessageStatus? status = null);

        bool UnlockIfLockedBeforeDateTime(Guid QID, DateTime lockTime);

        IList<Message> GetRetryExceededItems(Guid QID, int tries);

        bool MoveMessagesToQueue(IList<Message> msgs, Queue move_to_queue);

        IList<Message> GetItemsLastModifiedBefore(Guid QID, DateTime dateTime);
    }
}
