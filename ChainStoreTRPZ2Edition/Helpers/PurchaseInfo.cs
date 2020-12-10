using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public sealed class PurchaseInfo
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public DateTimeOffset PurchaseCreationTime { get; private set; }
        public double PriceAtPurchaseMoment { get; private set; }

        public PurchaseInfo(Product product, Purchase purchase)
        {
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateObject(purchase);
            ProductId = product.Id;
            ProductName = product.Name;
            PurchaseCreationTime = purchase.CreationTime.ToLocalTime();
            PriceAtPurchaseMoment = purchase.PriceAtPurchaseMoment;
        }
    }
}
