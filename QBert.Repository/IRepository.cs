using System;
namespace QBert.Repository
{
    public interface IRepository<T, K>
    {
        bool Init(Guid AccountID);
        bool Add(T a);
        bool Delete(K id);
        T Get(K id);
        bool Save(T a);
    }
}
