using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using QBert.Model;

namespace QBert.Controllers
{
    [RoutePrefix("Queue")]
    public class QueueController : BaseController
    {
        /// <summary>
        /// returns all queues for this account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Queue> Get()
        {
            return QueueRepository.GetAll();
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
            return QueueRepository.GetByName(Name);
        }

        /// <summary>
        /// add a queue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Queue Add([FromBody]Queue value)
        {
            value.AccountID = AccountID;
            if (!QueueRepository.Add(value)) return null;
            return value;
        }

        // PUT api/values/5
        [HttpPut]
        public bool Save(int id, [FromBody]Queue value)
        {
            return QueueRepository.Save(value);
        }

        // DELETE api/values/5
        [HttpDelete]
        public bool Delete(Guid ID)
        {
            return QueueRepository.Delete(ID);
        }
    }
}
