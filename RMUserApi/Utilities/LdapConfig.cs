using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;

namespace RMUserApi.Utilities
{
    /// <summary>
    /// web.config に定義されたLDAP接続情報を返す
    /// </summary>
    public static class LdapConfig
    {
        public static string Server
        {
            get { return ConfigurationManager.AppSettings["ldapServer"]; }
        }

        public static int Port
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ldapPort"]); }
        }

        public static string NamingContext
        {
            get { return ConfigurationManager.AppSettings["ldapNaimingContext"]; }
        }

        public static string SystemDn
        {
            get { return ConfigurationManager.AppSettings["ldapSystemDn"]; }
        }

        public static string SystemPassword
        {
            get { return ConfigurationManager.AppSettings["ldapSystemPassword"]; }
        }
    }
}