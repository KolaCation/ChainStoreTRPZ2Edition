using System;
using System.Collections.Generic;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class Category
    {
        private readonly List<Product> _products;

        public Category(Guid id, string name)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateString(name, 2, 60);
            Id = id;
            Name = name;
        }

        public Category(List<Product> products, Guid id, string name) : this(id, name)
        {
            DomainValidator.ValidateObject(products);
            _products = products;
        }

        public Guid Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    }
}