using System;
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
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Products.AddAsync(_productMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Product> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
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
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            if (Exists(item.Id))
            {
                var enState = context.Products.Update(_productMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            if (Exists(id))
            {
                var productDbModel = await context.Products.FindAsync(id);
                var storeProdRel =
                    await context.StoreProductRelation.FirstAsync(e =>
                        e.ProductDbModelId.Equals(productDbModel.Id));
                var enState1 = context.StoreProductRelation.Remove(storeProdRel);
                enState1.State = EntityState.Deleted;
                var enState2 = context.Products.Remove(productDbModel);
                enState2.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

    

        public async Task<Store> GetStoreOfSpecificProduct(Guid productId)
        {
            CustomValidator.ValidateId(productId);
            await using var context = new MyDbContext(_options);
            _storeMapper = new StoreMapper(context);
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
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateId(storeId);
            await using var context = new MyDbContext(_options);
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
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateId(storeId);
            await using var context = new MyDbContext(_options);
            var storeProdRelToRemove = await context.StoreProductRelation.FirstAsync(e =>
                e.ProductDbModelId.Equals(product.Id) && e.StoreDbModelId.Equals(storeId));
            var enState = context.StoreProductRelation.Remove(storeProdRelToRemove);
            enState.State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        #region Validations

        private async Task<bool> StoreProductRelationExists(Guid storeId, Guid productId)
        {
            CustomValidator.ValidateId(storeId);
            CustomValidator.ValidateId(productId);
            await using var context = new MyDbContext(_options);
            return await context.StoreProductRelation
                .AnyAsync(e => e.StoreDbModelId.Equals(storeId) && e.ProductDbModelId.Equals(productId));
        }
        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            using var context = new MyDbContext(_options);
            return context.Products.Any(item => item.Id.Equals(id));
        }

        #endregion
    }
}