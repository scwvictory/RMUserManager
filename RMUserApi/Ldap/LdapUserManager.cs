using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using RMUserApi.Models;

namespace RMUserApi.Ldap
{
    /// <summary>
    /// 自動的に UserStore に変更を保存する API に関連するユーザーを公開する
    /// UserManagerの派生クラス
    /// </summary>
    /// <remarks>
    /// 標準のUserManagerの場合、
    /// UserStore に IUserPasswordStore インタフェースの実装が必須となるが、
    /// LDAPでSSHAを使う場合、Saltによってハッシュが一定でなくなる為、
    /// LDAPへの接続時の認証として、dn/password を渡して、パスワード認証を行う。
    /// 
    /// そのため、FindAsyncによるパスワード認証処理を独自実装しなければならない。
    /// </remarks>
    public class LdapUserManager : UserManager<LdapUser>, IDisposable
    {
        public new LdapUserStore Store { get; set; }

        public LdapUserManager(IUserStore<LdapUser> store)
            : base(store)
        {
            Store = (LdapUserStore)store;
        }

        /// <summary>
        /// 指定されたユーザー名とパスワードを持つユーザーか、一致しない場合には null を返します。
        /// </summary>
        /// <param name="uid">ユーザーID(uid)</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task<LdapUser> FindAsync(string uid, string password)
        {
            try
            {
                //ユーザーストアの認証APIを利用
                return Store.Authenticate(uid, password);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}