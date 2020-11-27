using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class Purchase
    {
        public Guid Id { get; }
        public Guid ClientId { get; }
        public Guid ProductId { get; }
        public DateTimeOffset CreationTime { get; }
        public double PriceAtPurchaseMoment { get; }

        public Purchase(Guid id, Guid clientId, Guid productId, double priceAtPurchaseMoment)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateId(clientId);
            DomainValidator.ValidateId(productId);
            DomainValidator.ValidateNumber(priceAtPurchaseMoment, 0, 100_000_000);
            Id = id;
            ClientId = clientId;
            ProductId = productId;
            CreationTime = DateTimeOffset.UtcNow;
            PriceAtPurchaseMoment = priceAtPurchaseMoment;
        }

        public Purchase(Guid id, Guid clientId, Guid productId, DateTimeOffset creationTime, double priceAtPurchaseMoment) : this(id, clientId, productId, priceAtPurchaseMoment)
        {
            CreationTime = creationTime;
        }
    }
}