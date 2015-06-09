using System;
namespace BBQ.Repository
{
    public interface IRepository<T, K>
    {
        bool Init(Guid AccountID, string ConnectionString="");
        bool Add(T a);
        bool Delete(K id);
        T Get(K id);
        bool Save(T a);
    }
}
