using System;
using System.Collections.Generic;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public sealed class Store
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Location { get; }

        private readonly List<Category> _categories;
        public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

        public double Profit { get; private set; }

        public Store(Guid id, string name, string location, double profit)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateString(name, 2, 40);
            DomainValidator.ValidateString(location, 2, 40);
            DomainValidator.ValidateNumber(profit, 0, double.MaxValue);
            Id = id;
            Name = name;
            Location = location;
            Profit = profit;
            _categories = new List<Category>();
        }

        public Store(List<Category> categories, Guid id, string name, string location, double profit) : this(id, name, location, profit)
        {
            DomainValidator.ValidateObject(categories);
            _categories = categories;
        }

        public void GetProfit(double sum)
        {
            DomainValidator.ValidateNumber(sum, 0, 100_000_000);
            Profit += sum;
        }
    }
}