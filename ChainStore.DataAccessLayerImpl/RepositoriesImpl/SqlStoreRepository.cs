﻿using System;
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
    public class SqlStoreRepository : IStoreRepository
    {
        private readonly DbContextOptions<MyDbContext> _options;
        private StoreMapper _storeMapper;

        public SqlStoreRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Store item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            if (!Exists(item.Id))
            {
                var exists = await HasSameNameAndLocationAsync(item);
                if (!exists)
                {
                    var enState = await context.Stores.AddAsync(_storeMapper.DomainToDb(item));
                    enState.State = EntityState.Added;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<Store> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            if (Exists(id))
            {
                var storeDbModel = await context.Stores.FindAsync(id);
                return _storeMapper.DbToDomain(storeDbModel);
            }

            return null;
        }

        public async Task<IReadOnlyCollection<Store>> GetAll()
        {
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            var storeDbModels = await context.Stores.ToListAsync();
            var stores = (from storeDbModel in storeDbModels select _storeMapper.DbToDomain(storeDbModel)).ToList();
            return stores.AsReadOnly();
        }

        public async Task UpdateOne(Store item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            if (Exists(item.Id))
            {
                var exists = await HasSameNameAndLocationAsync(item);
                if (!exists)
                {
                    var enState = context.Stores.Update(_storeMapper.DomainToDb(item));
                    enState.State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            if (Exists(id))
            {
                var storeDbModel = await context.Stores.FindAsync(id);
                var enState = context.Stores.Remove(storeDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        #region Validations

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            using var context = new MyDbContext(_options);
            return context.Stores.Any(item => item.Id.Equals(id));
        }

        public async Task<IReadOnlyCollection<Product>> GetStoreSpecificProducts(Guid storeId)
        {
            CustomValidator.ValidateId(storeId);
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            var storeDbModel = await context.Stores.FindAsync(storeId);
            var store = _storeMapper.DbToDomain(storeDbModel);
            var products = new List<Product>();
            foreach (var category in store.Categories)
            {
                products.AddRange(category.Products);
            }

            return products;
        }

        public async Task<bool> HasSameNameAndLocationAsync(Store store)
        {
            await using var context = new MyDbContext(_options);
            if (store != null)
            {
                return await context.Stores.AnyAsync(e => e.Location.ToLower() == store.Location.ToLower() &&
                                                          e.Name.ToLower() == store.Name.ToLower() &&
                                                          !e.Id.Equals(store.Id));
            }

            return false;
        }

        public bool HasSameNameAndLocation(Store store)
        {
            using var context = new MyDbContext(_options);
            if (store != null)
            {
                return context.Stores.Any(e => e.Location.ToLower() == store.Location.ToLower() &&
                                               e.Name.ToLower() == store.Name.ToLower() &&
                                               !e.Id.Equals(store.Id));
            }

            return false;
        }

        #endregion
    }
}