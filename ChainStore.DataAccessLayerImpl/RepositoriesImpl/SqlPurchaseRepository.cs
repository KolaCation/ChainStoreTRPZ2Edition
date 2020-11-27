using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.RepositoriesImpl
{
    public class SqlPurchaseRepository : IPurchaseRepository
    {
        private readonly MyDbContext _context;
        private readonly PurchaseMapper _purchaseMapper;
        public SqlPurchaseRepository(MyDbContext context)
        {
            _context = context;
            _purchaseMapper = new PurchaseMapper();
        }

        public async Task AddOne(Purchase item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await _context.Purchases.AddAsync(_purchaseMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var purchaseDbModel = await _context.Purchases.FindAsync(id);
                var enState = _context.Purchases.Remove(purchaseDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Purchases.Any(item => item.Id.Equals(id));
        }

        public async Task<List<Purchase>> GetClientPurchases(Guid clientId)
        {
            var purchaseDbModels = await _context.Purchases.Where(p => p.ClientId.Equals(clientId)).ToListAsync();
            var purchases = (from purchaseDbModel in purchaseDbModels select _purchaseMapper.DbToDomain(purchaseDbModel)).ToList();
            return purchases;
        }
    }
}
