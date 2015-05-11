using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LinqToLdap.Mapping;
using RMUserApi.Models;
using RMUserApi.Utilities;

namespace RMUserApi.ModelMappers
{
    /// <summary>
    /// LDAP の inetOrgPerson オブジェクトと、LdapUser クラスとのマッピング
    /// </summary>
    public class LdapUserMapper : ClassMap<LdapUser>
    {
        public override IClassMap PerformMapping(string namingContext = null, string objectCategory = null, bool includeObjectCategory = true, IEnumerable<string> objectClasses = null, bool includeObjectClasses = true)
        {
            NamingContext(LdapConfig.NamingContext);

            ObjectClass("inetOrgPerson");

            DistinguishedName(x => x.DistinguishedName);
            Map(x => x.Id).Named("uid").ReadOnly();
            Map(x => x.UserName).Named("displayName");
            Map(x => x.LastName).Named("sn");
            Map(x => x.FirstName).Named("cn");
            Map(x => x.MailAddress).Named("mail");
            Map(x => x.OrganizationName).Named("organizationName");
            Map(x => x.Ou).Named("ou");

            return this;
        }
    }
}