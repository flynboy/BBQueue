using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Authorization
{
    public class BasicTokenParser : IAuthTokenParser
    {
        public bool Parse(string tokenString)
        {
            if (string.IsNullOrEmpty(tokenString)) return false;

            //expects accountId:secret
            //base 64 encoded
            try
            {
                byte[] data = Convert.FromBase64String(tokenString);
                string decodedString = Encoding.UTF8.GetString(data);

                if (decodedString.Contains(':'))
                {
                    var str_ar = decodedString.Split(':');
                    if (str_ar.Length == 2)
                    {
                        AccountID = new Guid(str_ar[0]);
                        Secret = str_ar[1];
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public Guid AccountID { get; private set; }
        public string Secret { get; private set; }
    }
}
