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
    [DirectorySchemaEx(ObjectClass = "organizationalUnit")]
    public class LdapOu
    {
        [DirectoryAttribute("ou", ReadOnly = true)]
        [Required, StringLength(200)]
        public string Name { get; set; }

        [DistinguishedName]
        public string DistinguishedName { get; set; }

        public void SetDistinguishedName()
        {
            DistinguishedName = string.Format("ou={0},{1}", Name, LdapConfig.NamingContext);
        }
    }
}