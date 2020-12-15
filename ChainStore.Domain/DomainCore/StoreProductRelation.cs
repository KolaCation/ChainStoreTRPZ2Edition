using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class StoreProductRelation
    {
        public StoreProductRelation(Guid storeId, Guid productId)
        {
            DomainValidator.ValidateId(storeId);
            DomainValidator.ValidateId(productId);
            StoreId = storeId;
            ProductId = productId;
        }

        public Guid StoreId { get; }
        public Guid ProductId { get; }
    }
}