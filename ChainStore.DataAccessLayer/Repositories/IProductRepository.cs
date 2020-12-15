using System;
using System.Threading.Tasks;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Store> GetStoreOfSpecificProduct(Guid productId);
        Task AddProductToStore(Product product, Guid storeId);
        Task DeleteProductFromStore(Product product, Guid storeId);
    }
}