using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class StoreCategoryRelation
    {
        public StoreCategoryRelation(Guid storeId, Guid categoryId)
        {
            DomainValidator.ValidateId(storeId);
            DomainValidator.ValidateId(categoryId);
            StoreId = storeId;
            CategoryId = categoryId;
        }

        public Guid StoreId { get; }
        public Guid CategoryId { get; }
    }
}