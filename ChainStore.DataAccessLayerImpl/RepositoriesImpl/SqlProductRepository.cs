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
    public class SqlProductRepository : IProductRepository
    {
        private readonly MyDbContext _context;
        private readonly ProductMapper _productMapper;
        private readonly StoreMapper _storeMapper;

        public SqlProductRepository(MyDbContext context)
        {
            _context = context;
            _productMapper = new ProductMapper();
            _storeMapper = new StoreMapper(context);
        }

        public async Task AddOne(Product item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await _context.Products.AddAsync(_productMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Product> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var productDbModel = await _context.Products.FindAsync(id);
                return _productMapper.DbToDomain(productDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Product>> GetAll()
        {
            var productDbModels = await _context.Products.ToListAsync();
            var products = (from productDbModel in productDbModels select _productMapper.DbToDomain(productDbModel))
                .ToList();
            return products.AsReadOnly();
        }

        public async Task UpdateOne(Product item)
        {
            CustomValidator.ValidateObject(item);
            if (Exists(item.Id))
            {
                var enState = _context.Products.Update(_productMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var productDbModel = await _context.Products.FindAsync(id);
                var storeProdRel =
                    await _context.StoreProductRelation.FirstAsync(e =>
                        e.ProductDbModelId.Equals(productDbModel.Id));
                await DeleteProductFromStore(_productMapper.DbToDomain(productDbModel), storeProdRel.StoreDbModelId);
                var enState = _context.Products.Remove(productDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

    

        public async Task<Store> GetStoreOfSpecificProduct(Guid productId)
        {
            CustomValidator.ValidateId(productId);
            var res = await _context.StoreProductRelation.FirstOrDefaultAsync(e => e.ProductDbModelId.Equals(productId));
            if (res != null)
            {
                var storeDbModelId = res.StoreDbModelId;
                var storeDbModel = await _context.Stores.FindAsync(storeDbModelId);
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
            if (!Exists(product.Id))
            {
                await AddOne(product);
            }
            var storeProdRelExists = await StoreProductRelationExists(storeId, product.Id);
            if (!storeProdRelExists)
            {
                var storeProdRel = new StoreProductDbModel(storeId, product.Id);
                await _context.StoreProductRelation.AddAsync(storeProdRel);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductFromStore(Product product, Guid storeId)
        {
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateId(storeId);
            var storeProdRelToRemove = await _context.StoreProductRelation.FirstAsync(e =>
                e.ProductDbModelId.Equals(product.Id) && e.StoreDbModelId.Equals(storeId));
            var enState = _context.StoreProductRelation.Remove(storeProdRelToRemove);
            enState.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        #region Validations

        private async Task<bool> StoreProductRelationExists(Guid storeId, Guid productId)
        {
            CustomValidator.ValidateId(storeId);
            CustomValidator.ValidateId(productId);
            return await _context.StoreProductRelation
                .AnyAsync(e => e.StoreDbModelId.Equals(storeId) && e.ProductDbModelId.Equals(productId));
        }
        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Products.Any(item => item.Id.Equals(id));
        }

        #endregion
    }
}