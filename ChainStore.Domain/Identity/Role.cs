using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.Util;

namespace ChainStore.Domain.Identity
{
    public sealed class Role
    {
        public Guid Id { get; private set; }
        public string RoleName { get; private set; }

        public Role(string roleName)
        {
            Id = Guid.NewGuid();
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
        }
    }
}
