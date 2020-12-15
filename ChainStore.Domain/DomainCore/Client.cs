using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.DomainCore
{
    public class Client
    {
        public Client(Guid id, string name, double balance)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateString(name, 2, 40);
            DomainValidator.ValidateNumber(balance, 0, 100_000_000);
            Id = id;
            Name = name;
            Balance = balance;
        }

        public Guid Id { get; }
        public string Name { get; private set; }
        public double Balance { get; private set; }

        public void UpdateName(string newName)
        {
            DomainValidator.ValidateString(newName, 2, 40);
            Name = newName;
        }

        public void ReplenishBalance(double sum)
        {
            DomainValidator.ValidateNumber(sum, 0, 100_000_000);
            Balance += sum;
        }

        public bool Charge(double sum)
        {
            DomainValidator.ValidateNumber(sum, 0, 100_000_000);
            if (Balance < sum) return false;
            Balance -= sum;
            return true;
        }
    }
}