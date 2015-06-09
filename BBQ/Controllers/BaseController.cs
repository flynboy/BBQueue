using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BBQ.Model;
using BBQ.Repository;

namespace BBQ.Controllers
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
                    _qRepo = RepositoryFactory.createQueueRepository();
                }
                return _qRepo;
            }
            set
            {
                _qRepo = value;
            }
        }

        private IMessageRepository _mRepo { get; set; }
        public IMessageRepository MessageRepository
        {
            get
            {
                if (_mRepo == null)
                {
                    _mRepo = RepositoryFactory.createMessageRepository();
                }
                return _mRepo;
            }
            set
            {
                _mRepo = value;
            }
        }

        private IAuthorizationRepository _aRepo { get; set; }
        public IAuthorizationRepository AccountRepository
        {
            get
            {
                if (_aRepo == null)
                {
                    _aRepo = RepositoryFactory.createAuthorizationRepository();
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
