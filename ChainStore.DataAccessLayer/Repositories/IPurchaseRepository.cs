using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface IPurchaseRepository : ICreateDeleteRepository<Purchase>
    {
        Task<List<Purchase>> GetClientPurchases(Guid clientId);
    }
}
