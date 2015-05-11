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
    /// LDAP の inetOrgPerson オブジェクトと、LdapUserPassword クラスとのマッピング
    /// </summary>
    public class LdapUserPasswordMapper : ClassMap<LdapUserPassword>
    {
        public override IClassMap PerformMapping(string namingContext = null, string objectCategory = null, bool includeObjectCategory = true, IEnumerable<string> objectClasses = null, bool includeObjectClasses = true)
        {
            NamingContext(LdapConfig.NamingContext);

            ObjectClass("inetOrgPerson");

            DistinguishedName(x => x.DistinguishedName);
            Map(x => x.Id).Named("uid").ReadOnly();
            Map(x => x.Password).Named("userPassword");

            return this;
        }
    }
}