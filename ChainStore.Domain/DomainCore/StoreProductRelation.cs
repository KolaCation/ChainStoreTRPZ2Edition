using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class StoreProductRelation
    {
        public Guid StoreId { get; }
        public Guid ProductId { get; }

        public StoreProductRelation(Guid storeId, Guid productId)
        {
            DomainValidator.ValidateId(storeId);
            DomainValidator.ValidateId(productId);
            StoreId = storeId;
            ProductId = productId;
        }
    }
}
