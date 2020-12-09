﻿using System;
using System.Collections.Generic;
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
    public class SqlProductRepository : IProductRepository
    {
        private readonly ProductMapper _productMapper;
        private StoreMapper _storeMapper;
        private readonly DbContextOptions<MyDbContext> _options;

        public SqlProductRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _productMapper = new ProductMapper();
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Product item)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await context.Products.AddAsync(_productMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Product> GetOne(Guid id)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var productDbModel = await context.Products.FindAsync(id);
                return _productMapper.DbToDomain(productDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Product>> GetAll()
        {
            await using var context = new MyDbContext(_options);
            var productDbModels = await context.Products.ToListAsync();
            var products = (from productDbModel in productDbModels select _productMapper.DbToDomain(productDbModel))
                .ToList();
            return products.AsReadOnly();
        }

        public async Task UpdateOne(Product item)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateObject(item);
            if (Exists(item.Id))
            {
                var enState = context.Products.Update(_productMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var productDbModel = await context.Products.FindAsync(id);
                var storeProdRel =
                    await context.StoreProductRelation.FirstAsync(e =>
                        e.ProductDbModelId.Equals(productDbModel.Id));
                await DeleteProductFromStore(_productMapper.DbToDomain(productDbModel),
                    storeProdRel.StoreDbModelId);
                var enState = context.Products.Remove(productDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

    

        public async Task<Store> GetStoreOfSpecificProduct(Guid productId)
        {
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
            CustomValidator.ValidateId(productId);
            var res = await context.StoreProductRelation.FirstOrDefaultAsync(e =>
                e.ProductDbModelId.Equals(productId));
            if (res != null)
            {
                var storeDbModelId = res.StoreDbModelId;
                var storeDbModel = await context.Stores.FindAsync(storeDbModelId);
                return _storeMapper.DbToDomain(storeDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task AddProductToStore(Product product, Guid storeId)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateId(storeId);
            if (!Exists(product.Id))
            {
                await AddOne(product);
            }

            var storeProdRelExists = await StoreProductRelationExists(storeId, product.Id);
            if (!storeProdRelExists)
            {
                var storeProdRel = new StoreProductDbModel(storeId, product.Id);
                await context.StoreProductRelation.AddAsync(storeProdRel);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteProductFromStore(Product product, Guid storeId)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateId(storeId);
            var storeProdRelToRemove = await context.StoreProductRelation.FirstAsync(e =>
                e.ProductDbModelId.Equals(product.Id) && e.StoreDbModelId.Equals(storeId));
            var enState = context.StoreProductRelation.Remove(storeProdRelToRemove);
            enState.State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        #region Validations

        private async Task<bool> StoreProductRelationExists(Guid storeId, Guid productId)
        {
            await using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(storeId);
            CustomValidator.ValidateId(productId);
            return await context.StoreProductRelation
                .AnyAsync(e => e.StoreDbModelId.Equals(storeId) && e.ProductDbModelId.Equals(productId));
        }
        public bool Exists(Guid id)
        {
            using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            return context.Products.Any(item => item.Id.Equals(id));
        }

        #endregion
    }
}