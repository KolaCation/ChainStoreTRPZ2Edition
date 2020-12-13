using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task AddCategoryToStore(Guid categoryId, Guid storeId);
        Task DeleteCategoryFromStore(Guid categoryId, Guid storeId);
        bool HasSameName(Category category);
        Task<bool> HasSameNameAsync(Category category);
    }
}
