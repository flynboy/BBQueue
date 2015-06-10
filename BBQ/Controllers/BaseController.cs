using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BBQ.Model;
using BBQ.Repository;
using System.Web;
using System.Text;

namespace BBQ.Controllers
{
    [Filters.ApiAuthorize]
    public class BaseController : ApiController
    {


        private Guid _authId { get; set; }
        public Guid AccountID
        {
            get
            {
                if (_authId == Guid.Empty)
                {
                    _authId = new Guid(User.Identity.Name);
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
                    _qRepo = RepositoryFactory.createQueueRepository(AccountID);
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
                    _mRepo = RepositoryFactory.createMessageRepository(AccountID);
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
                    _aRepo = RepositoryFactory.createAuthorizationRepository(AccountID);
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
