using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.Identity
{
    public sealed class UserRole
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        public UserRole(Guid userId, Guid roleId)
        {
            DomainValidator.ValidateId(userId);
            DomainValidator.ValidateId(roleId);
            UserId = userId;
            RoleId = roleId;
        }
    }
}
