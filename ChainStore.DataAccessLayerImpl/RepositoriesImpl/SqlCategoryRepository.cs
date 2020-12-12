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
        private CategoryMapper _categoryMapper;
        private readonly DbContextOptions<MyDbContext> _options;

        public SqlCategoryRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Category item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            _categoryMapper = new CategoryMapper(context);
            if (!Exists(item.Id))
            {
                var exists = await HasSameNameAsync(item);
                if (!exists)
                {
                    var enState = await context.Categories.AddAsync(_categoryMapper.DomainToDb(item));
                    enState.State = EntityState.Added;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<Category> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            _categoryMapper = new CategoryMapper(context);
            if (Exists(id))
            {
                var categoryDbModel = await context.Categories.FindAsync(id);
                return _categoryMapper.DbToDomain(categoryDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Category>> GetAll()
        {
            await using var context = new MyDbContext(_options);
            _categoryMapper = new CategoryMapper(context);
            var categoryDbModels = await context.Categories.ToListAsync();
            var categories =
                (from categoryDbModel in categoryDbModels select _categoryMapper.DbToDomain(categoryDbModel))
                .ToList();
            return categories.AsReadOnly();
        }

        public async Task UpdateOne(Category item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            _categoryMapper = new CategoryMapper(context);
            if (Exists(item.Id))
            {
                var exists = await HasSameNameAsync(item);
                if (!exists)
                {
                    var enState = context.Categories.Update(_categoryMapper.DomainToDb(item));
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
                var categoryDbModel = await context.Categories.FindAsync(id);
                var enState = context.Categories.Remove(categoryDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }


        public async Task AddCategoryToStore(Guid categoryId, Guid storeId)
        {
            CustomValidator.ValidateId(categoryId);
            CustomValidator.ValidateId(storeId);
            await using var context = new MyDbContext(_options);
            var exists = await StoreCategoryRelationExists(storeId, categoryId);
            if (!exists)
            {
                var storeCatRel = new StoreCategoryDbModel(storeId, categoryId);
                var enState = await context.StoreCategoryRelation.AddAsync(storeCatRel);
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryFromStore(Guid categoryId, Guid storeId)
        {
            CustomValidator.ValidateId(categoryId);
            CustomValidator.ValidateId(storeId);
            await using var context = new MyDbContext(_options);
            var exists = await StoreCategoryRelationExists(storeId, categoryId);
            if (exists)
            {
                var storeCatRelToRemove = await context.StoreCategoryRelation.FirstAsync(e =>
                    e.CategoryDbModelId.Equals(categoryId) && e.StoreDbModelId.Equals(storeId));
                var enState = context.StoreCategoryRelation.Remove(storeCatRelToRemove);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        #region Validations

        public async Task<bool> HasSameNameAsync(Category category)
        {
            await using var context = new MyDbContext(_options);
            if (category != null)
            {
                return await context.Categories.AnyAsync(e =>
                    e.Name.ToLower().Equals(category.Name.ToLower()) && !category.Id.Equals(e.Id));
            }
            else
            {
                return false;
            }
        }

        public bool HasSameName(Category category)
        {
            using var context = new MyDbContext(_options);
            if (category != null)
            {
                return context.Categories.Any(e =>
                    e.Name.ToLower().Equals(category.Name.ToLower()) && !category.Id.Equals(e.Id));
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> StoreCategoryRelationExists(Guid storeId, Guid categoryId)
        {
            await using var context = new MyDbContext(_options);
            return await context.StoreCategoryRelation
                .AnyAsync(e => e.CategoryDbModelId.Equals(categoryId) && e.StoreDbModelId.Equals(storeId));
        }

        public bool Exists(Guid id)
        {
            using var context = new MyDbContext(_options);
            CustomValidator.ValidateId(id);
            return context.Categories.Any(item => item.Id.Equals(id));
        }

        #endregion
    }
}