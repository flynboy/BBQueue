using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BBQ.Model;
using Newtonsoft.Json;

namespace BBQ.Controllers
{
    [RoutePrefix("Queue")]
    public class QueueController : BaseController
    {
        public bool SuppressTimeoutProcessing { get; set; }
        private static DateTime LastTimeoutProcessing { get; set; }

        static QueueController()
        {
            LastTimeoutProcessing = DateTime.Now.AddMinutes(-15);
        }

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
            if (!SuppressTimeoutProcessing && LastTimeoutProcessing.AddSeconds(15) < DateTime.Now)
            {
                ProcessTimeouts(QID);
            }
            return MessageRepository.GetNextAndLock(QID);
        }

        [HttpDelete, Route("{QID}/Message/{ID}")]
        public bool RemoveMessage(Guid QID, Guid ID)
        {
            return MessageRepository.Delete(ID);
        }

        #endregion

        #region timeout processing

        [HttpPost, Route("{QID}/_ProcessTimeouts")]
        public bool ProcessTimeouts(Guid QID)
        {
            var q = QueueRepository.Get(QID);
            if (q == null) return false;

            //unlock locked timed out items
            if (q.LockTimeout > 0)
            {
                if (!MessageRepository.UnlockIfLockedBeforeDateTime(QID, DateTime.Now.AddSeconds(q.LockTimeout * -1))) return false;
            }

            //send retry exceed to SYSTEM_POISON
            if (q.MaxAttempts >= 0)
            {
                var poison_msgs = MessageRepository.GetRetryExceededItems(QID, q.MaxAttempts);
                if (poison_msgs.Count > 0)
                {
                    //move these to the dead queue
                    var poison_queue_name = "SYSTEM_POISON";
                    var poison_queue = QueueRepository.GetByName(poison_queue_name);
                    if (poison_queue == null)
                    {
                        if (!QueueRepository.Add(new Queue()
                        {
                            AccountID = AccountID,
                            Name = poison_queue_name,
                            LockTimeout = -1,
                            MaxAttempts = -1,
                            TimeToLive = -1
                        })) return false;
                        poison_queue = QueueRepository.GetByName(poison_queue_name);
                        if (poison_queue == null) return false;
                    }
                    if (!MessageRepository.MoveMessagesToQueue(poison_msgs, poison_queue)) return false;
                }
            }

            if (q.TimeToLive > 0)
            {
                //send expired items to SYSTEM_EXPIRED queue     
                var expired_msgs = MessageRepository.GetItemsLastModifiedBefore(QID, DateTime.Now.AddSeconds(q.TimeToLive * -1));
                if (expired_msgs.Count > 0)
                {
                    //move these to the expired queue
                    var expired_queue_name = "SYSTEM_EXPIRED";
                    var expired_queue = QueueRepository.GetByName(expired_queue_name);
                    if (expired_queue == null)
                    {
                        if (!QueueRepository.Add(new Queue()
                        {
                            AccountID = AccountID,
                            Name = expired_queue_name,
                            LockTimeout = -1,
                            MaxAttempts = -1,
                            TimeToLive = -1
                        })) return false;
                        expired_queue = QueueRepository.GetByName(expired_queue_name);
                        if (expired_queue == null) return false;
                    }
                    if (!MessageRepository.MoveMessagesToQueue(expired_msgs, expired_queue)) return false;
                }
            }

            return true;
        }
        #endregion
    }
}
