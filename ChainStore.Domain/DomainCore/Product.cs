using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class Product
    {
        public Product(Guid id, string name, double priceInUAH, ProductStatus productStatus, Guid categoryId)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateId(categoryId);
            DomainValidator.ValidateString(name, 2, 40);
            DomainValidator.ValidateNumber(priceInUAH, 0, 100_000_000);
            Id = id;
            Name = name;
            PriceInUAH = priceInUAH;
            ProductStatus = productStatus;
            CategoryId = categoryId;
        }

        public Guid Id { get; }
        public string Name { get; }
        public double PriceInUAH { get; }
        public ProductStatus ProductStatus { get; private set; }
        public Guid CategoryId { get; }

        public void ChangeStatus(ProductStatus productStatus)
        {
            ProductStatus = productStatus;
        }
    }
}