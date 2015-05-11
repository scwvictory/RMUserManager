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
    /// LDAP の organizationalUnit オブジェクトと、LdapOu クラスとのマッピング
    /// </summary>
    public class LdapOuMapper : ClassMap<LdapOu>
    {
        public override IClassMap PerformMapping(string namingContext = null, string objectCategory = null, bool includeObjectCategory = true, IEnumerable<string> objectClasses = null, bool includeObjectClasses = true)
        {
            NamingContext(LdapConfig.NamingContext);

            ObjectClass("organizationalUnit");

            DistinguishedName(x => x.DistinguishedName);
            Map(x => x.Name).Named("ou").ReadOnly();
            return this;
        }
    }
}