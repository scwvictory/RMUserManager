using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace RMUserApi.Models
{
    public class RMUserDbInitializer : CreateDatabaseIfNotExists<RMUserDbContext>
    {
        protected override void Seed(RMUserDbContext context)
        {
            var roles = new List<LdapUserRole> {
                new LdapUserRole
                {
                    Name = "Administrator",
                },
            };
            context.LdapUserRoles.AddRange(roles);
            context.SaveChanges();
            base.Seed(context);
        }
    }
}