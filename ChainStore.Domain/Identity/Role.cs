using System;

namespace ChainStore.Domain.Identity
{
    public sealed class Role
    {
        public Role(string roleName)
        {
            Id = Guid.NewGuid();
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
        }

        public Guid Id { get; private set; }
        public string RoleName { get; private set; }
    }
}