using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.RepositoriesImpl
{
    public class SqlStoreRepository : IStoreRepository
    {
        private readonly MyDbContext _context;
        private readonly StoreMapper _storeMapper;

        public SqlStoreRepository(MyDbContext context)
        {
            _context = context;
            _storeMapper = new StoreMapper(context);
        }

        public async Task AddOne(Store item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var exists = await HasSameNameAndLocation(item);
                if (!exists)
                {
                    var enState = await _context.Stores.AddAsync(_storeMapper.DomainToDb(item));
                    enState.State = EntityState.Added;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<Store> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var storeDbModel = await _context.Stores.FindAsync(id);
                await _context.Entry(storeDbModel).Collection(st => st.CategoryDbModels).LoadAsync();
                return _storeMapper.DbToDomain(storeDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Store>> GetAll()
        {
            var storeDbModels = await _context.Stores.ToListAsync();
            var stores = (from storeDbModel in storeDbModels select _storeMapper.DbToDomain(storeDbModel)).ToList();
            return stores.AsReadOnly();
        }

        public async Task UpdateOne(Store item)
        {
            CustomValidator.ValidateObject(item);
            if (Exists(item.Id))
            {
                var exists = await HasSameNameAndLocation(item);
                if (!exists)
                {
                    DetachService.Detach<StoreDbModel>(_context, item.Id);
                    var enState = _context.Stores.Update(_storeMapper.DomainToDb(item));
                    enState.State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
        
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var storeDbModel = await _context.Stores.FindAsync(id);
                var enState = _context.Stores.Remove(storeDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }


        #region Validations

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Stores.Any(item => item.Id.Equals(id));
        }

        public async Task<IReadOnlyCollection<Product>> GetStoreSpecificProducts(Guid storeId)
        {
            CustomValidator.ValidateId(storeId);
            var storeDbModel = await _context.Stores.FindAsync(storeId);
            var store = _storeMapper.DbToDomain(storeDbModel);
            var products = new List<Product>();
            foreach (var category in store.Categories)
            {
                products.AddRange(category.Products);
            }

            return products;
        }

        private async Task<bool> HasSameNameAndLocation(Store store)
        {
            if (store != null)
            {
                return await _context.Stores.AnyAsync(e => e.Location.ToLower().Equals(store.Location.ToLower()) &&
                                                           e.Name.ToLower().Equals(store.Name.ToLower()) &&
                                                           !e.Id.Equals(store.Id));
            }

            return false;
        }

        #endregion
    }
}