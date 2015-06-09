using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using QBert.Model;
using QBert.Repository;

namespace QBert.Controllers
{
    public class BaseController : ApiController
    {
        //todo:  authorize
        private Guid _authId { get; set; }
        public Guid AccountID
        {
            get
            {
                if (_authId == null)
                {
                    return Guid.Empty; //todo:  auth here
                }
                return _authId;
            }
            set
            {
                _authId = value;
            }
        }

        #region Repositories

        private IQueueRepository _qRepo { get; set; }
        public IQueueRepository QueueRepository
        {
            get
            {
                if(_qRepo==null)
                {
                    //todo:  load from config / injection
                }
                return _qRepo;
            }
            set
            {
                _qRepo = value;
            }
        }

        private IAuthorizationRepository _aRepo { get; set; }
        public IAuthorizationRepository AccountRepository
        {
            get
            {
                if (_aRepo == null)
                {
                    //todo:  load from config / injection
                }
                return _aRepo;
            }
            set
            {
                _aRepo = value;
            }
        }

        #endregion
    }
}
