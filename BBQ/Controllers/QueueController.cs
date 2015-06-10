using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BBQ.Model;

namespace BBQ.Controllers
{
    [RoutePrefix("Queue")]
    public class QueueController : BaseController
    {
        /// <summary>
        /// Returns a queue by id.
        /// </summary>
        [HttpGet, Route("{ID}")]
        public Queue Get(Guid ID)
        {
            return QueueRepository.Get(ID);
        }

        /// <summary>
        /// Returns a queue by name.
        /// </summary>
        [HttpGet, Route("")]
        public Queue Get(string Name)
        {
            var q = QueueRepository.GetByName(Name);
            if (q == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return q;
        }

        /// <summary>
        /// returns all queues for this account
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public IEnumerable<Queue> Get()
        {
            return QueueRepository.GetAll();
        }
        
        /// <summary>
        /// add a queue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        public Queue Add([FromBody]Queue value)
        {
            value.AccountID = AccountID;
            if (!QueueRepository.Add(value)) throw new HttpResponseException(HttpStatusCode.InternalServerError);
            return value;
        }

        // PUT api/values/5
        [HttpPut, Route("{ID}")]
        public bool Save(Guid ID, [FromBody]Queue value)
        {
            return QueueRepository.Save(value);
        }

        // DELETE api/values/5
        [HttpDelete, Route("{ID}")]
        public bool Delete(Guid ID)
        {
            return QueueRepository.Delete(ID);
        }

        #region messages

        [HttpPost, Route("{QID}/Message")]
        public Message AddMessage(Guid QID, [FromBody]object payload)
        {
            var q = Get(QID);

            var msg = new Message(q)
            {
                Payload = payload
            };

            if(!MessageRepository.Add(msg))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return msg;
        }

        [HttpGet, Route("{QID}/Message")]
        public Message GetNextMessage(Guid QID)
        {
            var q = Get(QID);
            return MessageRepository.GetNextAndLock(q.ID);
        }

        [HttpDelete, Route("{QID}/Message/{ID}")]
        public bool RemoveMessage(Guid QID, Guid ID)
        {
            var q = Get(QID);

            //get it to make sure we can access it
            var m = MessageRepository.Get(ID);
            if(m==null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return MessageRepository.Delete(ID);
        }

        #endregion
    }
}
