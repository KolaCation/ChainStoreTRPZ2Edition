using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface IStoreRepository : IRepository<Store>
    {
        Task<IReadOnlyCollection<Product>> GetStoreSpecificProducts(Guid storeId);

        bool HasSameNameAndLocation(Store store);
        Task<bool> HasSameNameAndLocationAsync(Store store);
    }
}
