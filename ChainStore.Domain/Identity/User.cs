using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.Identity
{
    public sealed class User
    {
        public User(Guid id, string userName, string email, Guid clientId)
        {
            DomainValidator.ValidateId(id);
            DomainValidator.ValidateId(clientId);
            Id = id;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            ClientId = clientId;
        }

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string HashedPassword { get; private set; }
        public Guid ClientId { get; private set; }

        public void SetHashedPassword(string hashedPassword)
        {
            if (!string.IsNullOrEmpty(hashedPassword)) HashedPassword = hashedPassword;
        }
    }
}