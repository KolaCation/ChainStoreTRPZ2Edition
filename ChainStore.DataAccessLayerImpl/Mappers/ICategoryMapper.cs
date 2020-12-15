using System;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.Domain.DomainCore;

namespace ChainStore.DataAccessLayerImpl.Mappers
{
    internal interface ICategoryMapper : IMapper<Category, CategoryDbModel>
    {
        Category DbToDomainStoreSpecificProducts(CategoryDbModel item, Guid storeId);
    }
}