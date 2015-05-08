using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LinqToLdap;
using System.DirectoryServices.Protocols;
using System.Net;
using RMUserApi.Ldap;
using RMUserApi.Utilities;

namespace RMUserApi.Ldap
{
    /// <summary>
    /// LDAPとの接続を司るコンテキストクラス
    /// </summary>
    public class LdapContext : IDisposable
    {
        //LDAPへの接続
        public IDirectoryContext Context { get; set; }

        /// <summary>
        /// LDAPサーバへ接続する
        /// </summary>
        /// <param name="dn">接続先の一意名</param>
        /// <param name="password">パスワード</param>
        public void Connect(string dn = null, string password = null)
        {
            try
            {
                //一意名が未指定の場合、システム用一意名・パスワードを取得
                if (dn == null)
                {
                    dn = LdapConfig.SystemDn;
                    password = LdapConfig.SystemPassword;
                }

                //設定情報の生成
                LdapConfiguration config = new LdapConfiguration();
                config
                    .MaxPageSizeIs(10000)
                    .AddMapping(new LdapUserMapper())
                    .ConfigureFactory(LdapConfig.Server)
                    .AuthenticateBy(AuthType.Basic)
                    .AuthenticateAs(new NetworkCredential(dn, password))
                    .ProtocolVersion(3)
                    .UsePort(LdapConfig.Port);

                //接続
                this.Context = new DirectoryContext(config);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// アンマネージ リソースの解放
        /// </summary>
        public void Dispose()
        {
            this.Context.Dispose();
        }
    }
}