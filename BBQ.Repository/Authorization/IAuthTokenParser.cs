using System;
namespace BBQ.Repository.Authorization
{
    public interface IAuthTokenParser
    {
        Guid AccountID { get; }
        bool Parse(string tokenString);
        string Secret { get; }
    }
}
