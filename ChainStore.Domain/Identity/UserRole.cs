using System;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.Identity
{
    public sealed class UserRole
    {
        public UserRole(Guid userId, Guid roleId)
        {
            DomainValidator.ValidateId(userId);
            DomainValidator.ValidateId(roleId);
            UserId = userId;
            RoleId = roleId;
        }

        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
    }
}