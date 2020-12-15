using System;
using System.Collections.Generic;
using System.Linq;
using ChainStore.Shared.Util;

namespace ChainStore.DataAccessLayerImpl.DbModels
{
    internal sealed class StoreDbModel
    {
        private readonly List<StoreCategoryDbModel> _storeCategoryRelation;

        private readonly List<StoreProductDbModel> _storeProductRelation;

        public StoreDbModel(Guid id, string name, string location, double profit)
        {
            CustomValidator.ValidateId(id);
            CustomValidator.ValidateString(name, 2, 60);
            CustomValidator.ValidateString(location, 2, 60);
            CustomValidator.ValidateNumber(profit, 0, double.MaxValue);
            Id = id;
            Name = name;
            Location = location;
            Profit = profit;
            _storeCategoryRelation = new List<StoreCategoryDbModel>();
            _storeProductRelation = new List<StoreProductDbModel>();
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public IReadOnlyCollection<StoreCategoryDbModel> StoreCategoryRelation => _storeCategoryRelation.AsReadOnly();
        public IReadOnlyCollection<StoreProductDbModel> StoreProductRelation => _storeProductRelation.AsReadOnly();

        public IReadOnlyCollection<CategoryDbModel> CategoryDbModels => GetStoreSpecificCategories();

        public double Profit { get; private set; }

        private IReadOnlyCollection<CategoryDbModel> GetStoreSpecificCategories()
        {
            return (from storeCatRel in _storeCategoryRelation
                where storeCatRel.StoreDbModelId.Equals(Id)
                select storeCatRel.CategoryDbModel).ToList().AsReadOnly();
        }
    }
}