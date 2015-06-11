﻿using BBQ.Model;
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
    }
}
