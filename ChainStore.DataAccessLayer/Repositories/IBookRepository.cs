using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface IBookRepository : ICreateDeleteRepository<Book>
    {
        Task<List<Book>> GetClientBooks(Guid clientId);
        Task CheckBooksForExpiration();
    }
}
