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
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly MyDbContext _context;
        private readonly CategoryMapper _categoryMapper;

        public SqlCategoryRepository(MyDbContext context)
        {
            _context = context;
            _categoryMapper = new CategoryMapper(context);
        }

        public async Task AddOne(Category item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var exists = await HasSameName(item);
                if (!exists)
                {
                    var enState = await _context.Categories.AddAsync(_categoryMapper.DomainToDb(item));
                    enState.State = EntityState.Added;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<Category> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var categoryDbModel = await _context.Categories.FindAsync(id);
                return _categoryMapper.DbToDomain(categoryDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Category>> GetAll()
        {
            var categoryDbModels = await _context.Categories.ToListAsync();
            var categories =
                (from categoryDbModel in categoryDbModels select _categoryMapper.DbToDomain(categoryDbModel)).ToList();
            return categories.AsReadOnly();
        }

        public async Task UpdateOne(Category item)
        {
            CustomValidator.ValidateObject(item);
            if (Exists(item.Id))
            {
                var exists = await HasSameName(item);
                if (!exists)
                {
                    DetachService.Detach<CategoryDbModel>(_context, item.Id);
                    var enState = _context.Categories.Update(_categoryMapper.DomainToDb(item));
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
                var categoryDbModel = await _context.Categories.FindAsync(id);
                var enState = _context.Categories.Remove(categoryDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

    

        public async Task AddCategoryToStore(Guid categoryId, Guid storeId)
        {
            CustomValidator.ValidateId(categoryId);
            CustomValidator.ValidateId(storeId);
            var exists = await StoreCategoryRelationExists(storeId, categoryId);
            if (!exists)
            {
                var storeCatRel = new StoreCategoryDbModel(storeId, categoryId);
                var enState = await _context.StoreCategoryRelation.AddAsync(storeCatRel);
                enState.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryFromStore(Guid categoryId, Guid storeId)
        {
            CustomValidator.ValidateId(categoryId);
            CustomValidator.ValidateId(storeId);
            var exists = await StoreCategoryRelationExists(storeId, categoryId);
            if (exists)
            {
                var storeCatRelToRemove = await _context.StoreCategoryRelation.FirstAsync(e =>
                    e.CategoryDbModelId.Equals(categoryId) && e.StoreDbModelId.Equals(storeId));
                var enState = _context.StoreCategoryRelation.Remove(storeCatRelToRemove);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

        #region Validations

        private async Task<bool> HasSameName(Category category)
        {
            if (category != null)
            {
                return await _context.Categories.AnyAsync(e => e.Name.ToLower().Equals(category.Name.ToLower()) && !category.Id.Equals(e.Id));
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> StoreCategoryRelationExists(Guid storeId, Guid categoryId)
        {
            return await _context.StoreCategoryRelation
                .AnyAsync(e => e.CategoryDbModelId.Equals(categoryId) && e.StoreDbModelId.Equals(storeId));
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Categories.Any(item => item.Id.Equals(id));
        }

        #endregion
    }
}