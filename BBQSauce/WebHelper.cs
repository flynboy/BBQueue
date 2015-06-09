using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BBQSauce
{
    public class WebHelper
    {
        public WebHelper(string BaseUri, string AccountID, string Secret)
        {
            this.BaseUri = BaseUri;
            this.AccountID = AccountID;
            this.Secret = Secret;
        }

        public T Get<T>(string resource)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "GET";

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode < HttpStatusCode.OK || (int)response.StatusCode >= 300)
                    {
                        return default(T);
                    }

                    return new JsonSerializer().Deserialize<T>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
                }
            }
            catch(WebException ex)
            {
                var r = ex.Response as HttpWebResponse;
                if(r==null || r.StatusCode != HttpStatusCode.NotFound)
                {
                    throw ex;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public T Post<T>(string resource, object data)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode < HttpStatusCode.OK || (int)response.StatusCode >= 300)
                {
                    return default(T);
                }

                return new JsonSerializer().Deserialize<T>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
            }
        }

        public bool Post(string resource, object data)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                return response.StatusCode >= HttpStatusCode.OK && (int)response.StatusCode < 300;
            }
        }

        public bool Put(string resource, object data)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                return response.StatusCode >= HttpStatusCode.OK && (int)response.StatusCode < 300;
            }
        }

        public T Put<T>(string resource, object data)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode < HttpStatusCode.OK || (int)response.StatusCode >= 300)
                {
                    return default(T);
                }

                return new JsonSerializer().Deserialize<T>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
            }
        }

        public bool Delete(string resource)
        {
            resource = BaseUri + resource.Replace("//", "/").Replace(":/", "://");
            WebRequest request = WebRequest.Create(resource);
            request.Headers["Authorization"] = token;
            request.Method = "DELETE";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                return response.StatusCode >= HttpStatusCode.OK && (int)response.StatusCode < 300;
            }
        }

        private string token
        {
            get
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", AccountID, Secret)));
            }
        }


        private string BaseUri { get; set; }
        private string AccountID { get; set; }
        private string Secret { get; set; }
    }
}
