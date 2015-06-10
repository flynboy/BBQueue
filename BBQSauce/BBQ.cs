using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQSauce
{
    public class BBQ
    {
        public Queue CreateQueue(string Name)
        {
            return api.Post<Queue>("Queue", new Queue() { Name = Name });
        }

        public Queue GetQueue(string Name)
        {
            return api.Get<Queue>("Queue?name=" + Name);
        }

        public Queue GetOrCreateQueue(string Name)
        {
            var q = GetQueue(Name);
            if(q==null)
            {
                q = CreateQueue(Name);
            }
            return q;
        }

        public IEnumerable<Queue> GetAllQueues()
        {
            return api.Get<List<Queue>>("Queue");
        }

        public Message<T> Enqueue<T>(Queue q, T o)
        {
            return api.Post<Message<T>>("Queue/" + q.ID.ToString() + "/Message", o);
        }

        public Message<T> DeQueue<T>(Queue q)
        {
            return api.Get<Message<T>>("Queue/" + q.ID.ToString() + "/Message");
        }

        public bool MessageProcessed<T>(Queue q, Message<T> msg)
        {
            return api.Delete("Queue/" + q.ID.ToString() + "/Message/" + msg.ID.ToString());
        }

        private WebHelper api
        {
            get
            {
                var ret = new WebHelper(BaseUri, AccountID, Secret);
                return ret;
            }
        }

        #region settings

        private static string _baseUri;
        public static string BaseUri
        {
            get
            {
                if (_baseUri == null)
                {
                    _baseUri = ConfigurationManager.AppSettings.Get("BBQ.BaseUrl");
                }

                var ret = _baseUri;
                if (!string.IsNullOrEmpty(_baseUri) && !_baseUri.EndsWith(@"/")) _baseUri += @"/";
                return _baseUri;
            }
            set
            {
                _baseUri = value;
            }
        }

        private static string _accountID;
        public static string AccountID
        {
            get
            {
                if (_accountID == null)
                {
                    _accountID = ConfigurationManager.AppSettings.Get("BBQ.AccountID");
                }

                return _accountID;
            }
            set
            {
                _accountID = value;
            }
        }

        private static string _secret;
        public static string Secret
        {
            get
            {
                if (_secret == null)
                {
                    _secret = ConfigurationManager.AppSettings.Get("BBQ.Secret");
                }

                return _secret;
            }
            set
            {
                _secret = value;
            }
        }
        #endregion        
    }
}
