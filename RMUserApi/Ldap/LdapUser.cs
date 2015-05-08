using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using LinqToLdap.Mapping;
using RMUserApi.Attributes;
using RMUserApi.Utilities;

namespace RMUserApi.Ldap
{
//    [DirectorySchemaEx(ObjectCategory = "inetOrgPerson", ObjectClass = "LdapUser")]
    [DirectorySchemaEx(ObjectClass = "inetOrgPerson")]
    public class LdapUser : IUser
    {
        [DirectoryAttribute("uid", ReadOnly = true)]
        public string Id { get; set; }

        [DirectoryAttribute("displayName")]
        public string UserName { get; set; }

        [DistinguishedName]
        public string DistinguishedName { get; set; }

        [DirectoryAttribute("sn")]
        public string LastName { get; set; }

        [DirectoryAttribute("cn")]
        public string FirstName { get; set; }

        [DirectoryAttribute("mail")]
        public string MailAddress { get; set; }

        [DirectoryAttribute("organizationName")]
        public string OrganizationName { get; set; }

        public void SetDistinguishedName(string ou = null)
        {
            if (ou == null)
                DistinguishedName = string.Format("cn={0},{1}", Id, LdapConfig.NamingContext);
            else
                DistinguishedName = string.Format("cn={0},ou={1},{2}", Id, ou, LdapConfig.NamingContext);
        }

        public void SetDisplayName()
        {
            if (OrganizationName == null)
                UserName = LastName + FirstName;
            else
                UserName = OrganizationName + " " + LastName + FirstName;
        }
    }
}