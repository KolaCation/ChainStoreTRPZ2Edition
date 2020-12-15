using System;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public sealed class PurchaseInfo
    {
        public PurchaseInfo(Product product, Purchase purchase)
        {
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateObject(purchase);
            ProductId = product.Id;
            ProductName = product.Name;
            PurchaseCreationTime = purchase.CreationTime.ToLocalTime();
            PriceAtPurchaseMoment = purchase.PriceAtPurchaseMoment;
        }

        public Guid ProductId { get; }
        public string ProductName { get; }
        public DateTimeOffset PurchaseCreationTime { get; }
        public double PriceAtPurchaseMoment { get; }
    }
}