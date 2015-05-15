namespace RMUserApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using System.Collections.Generic;
    using RMUserApi.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RMUserApi.Models.RMUserDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RMUserApi.Models.RMUserDbContext context)
        {
            var roles = new List<LdapUserRole> {
                new LdapUserRole
                {
                    Name = "Administrator",
                },
            };
            context.LdapUserRoles.AddRange(roles);
            context.SaveChanges();
        }
    }
}
