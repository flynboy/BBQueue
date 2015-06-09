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
    public class BaseController : ApiController
    {
        private Guid _authId { get; set; }
        public Guid AccountID
        {
            get
            {
                if (_authId == Guid.Empty)
                {
                    if (HttpContext.Current != null)
                    {
                        var headers = HttpContext.Current.Request.Headers;
                        if(headers!=null)
                        {
                            var Auth = headers["Authorization"];
                            if(!string.IsNullOrEmpty(Auth))
                            {
                                //expects accountId:secret
                                //base 64 encoded
                                byte[] data = Convert.FromBase64String(Auth);
                                string decodedString = Encoding.UTF8.GetString(data);

                                if(decodedString.Contains(':'))
                                {
                                    var str_ar = decodedString.Split(':');
                                    if (str_ar.Length == 2)
                                    {                                        
                                        var accountID = new Guid(str_ar[0]);
                                        var secret = str_ar[1];

                                        //todo:  verify accountID and secret here                                        

                                        AccountID = accountID;
                                        return accountID;
                                    }
                                }
                            }
                        }
                    }
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
