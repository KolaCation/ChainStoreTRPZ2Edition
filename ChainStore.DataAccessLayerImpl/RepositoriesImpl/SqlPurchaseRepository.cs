using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.RepositoriesImpl
{
    public class SqlPurchaseRepository : IPurchaseRepository
    {
        private readonly PurchaseMapper _purchaseMapper;
        private readonly DbContextOptions<MyDbContext> _options;

        public SqlPurchaseRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _purchaseMapper = new PurchaseMapper();
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Purchase item)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await context.Purchases.AddAsync(_purchaseMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var purchaseDbModel = await context.Purchases.FindAsync(id);
                var enState = context.Purchases.Remove(purchaseDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            return context.Purchases.Any(item => item.Id.Equals(id));
        }

        public async Task<List<Purchase>> GetClientPurchases(Guid clientId)
        {
            await using var context = new MyDbContext(_options);
            var purchaseDbModels = await context.Purchases.Where(p => p.ClientId.Equals(clientId)).ToListAsync();
            var purchases = (from purchaseDbModel in purchaseDbModels
                select _purchaseMapper.DbToDomain(purchaseDbModel)).ToList();
            return purchases;
        }
    }
}
