using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace RMUserApi.Models
{
    public class RMUserDbContext : DbContext
    {
        public RMUserDbContext()
            : base("name=RMUserDbConnection")
        {
        }

        public virtual DbSet<LdapUserRole> LdapUserRoles { get; set; }
        public virtual DbSet<LdapUserRoleMember> LdapUserRoleMembers { get; set; }
    }
}