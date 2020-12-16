﻿using System.Linq;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.Mappers
{
    internal sealed class StoreMapper : IMapper<Store, StoreDbModel>
    {
        private readonly CategoryMapper _categoryMapper;
        private readonly MyDbContext _context;

        public StoreMapper(MyDbContext context)
        {
            _context = context;
            _categoryMapper = new CategoryMapper(context);
        }

        public StoreDbModel DomainToDb(Store item)
        {
            CustomValidator.ValidateObject(item);
            return new StoreDbModel(item.Id, item.Name, item.Location, item.Profit);
        }

        public Store DbToDomain(StoreDbModel item)
        {
            CustomValidator.ValidateObject(item);
            var storeDbModel = _context.Stores.Where(e => e.Id.Equals(item.Id))
                .Include(e => e.StoreCategoryRelation)
                .ThenInclude(e => e.CategoryDbModel)
                .Include(e => e.StoreProductRelation)
                .ThenInclude(e => e.ProductDbModel).FirstOrDefault();
            return new Store(
                (from categoryDbModel in storeDbModel.CategoryDbModels
                    select _categoryMapper.DbToDomainStoreSpecificProducts(categoryDbModel, item.Id)).ToList(),
                item.Id,
                item.Name,
                item.Location,
                item.Profit);
        }
    }
}