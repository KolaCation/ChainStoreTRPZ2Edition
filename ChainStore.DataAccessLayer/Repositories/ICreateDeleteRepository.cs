using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface ICreateDeleteRepository<T>
    {
        Task AddOne(T item);
        Task DeleteOne(Guid id);
        bool Exists(Guid id);
    }
}
