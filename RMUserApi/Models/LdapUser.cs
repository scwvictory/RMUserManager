using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using LinqToLdap.Mapping;
using RMUserApi.Attributes;
using RMUserApi.Utilities;

namespace RMUserApi.Models
{
    [DirectorySchemaEx(ObjectClass = "inetOrgPerson")]
    public class LdapUser : IUser
    {
        [DirectoryAttribute("uid", ReadOnly = true)]
        [Required, StringLength(200)]
        public string Id { get; set; }

        [DirectoryAttribute("displayName")]
        public string UserName { get; set; }

        [DistinguishedName]
        public string DistinguishedName { get; set; }

        [DirectoryAttribute("sn")]
        [Required, StringLength(100)]
        public string LastName { get; set; }

        [DirectoryAttribute("cn")]
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [DirectoryAttribute("mail")]
        [Required, DataType(DataType.EmailAddress), StringLength(200)]
        public string MailAddress { get; set; }

        [DirectoryAttribute("organizationName")]
        [StringLength(100)]
        public string OrganizationName { get; set; }

        [DirectoryAttribute("ou")]
        [StringLength(100)]
        public string Ou { get; set; }

        public void SetDistinguishedName()
        {
            if (Ou == null)
                DistinguishedName = string.Format("uid={0},{1}", Id, LdapConfig.NamingContext);
            else
                DistinguishedName = string.Format("uid={0},ou={1},{2}", Id, Ou, LdapConfig.NamingContext);
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