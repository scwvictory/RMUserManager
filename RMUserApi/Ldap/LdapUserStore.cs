using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using System.DirectoryServices.Protocols;
using Microsoft.AspNet.Identity;
using RMUserApi.Ldap;

namespace RMUserApi.Ldap
{
    /// <summary>
    /// ユーザー管理 API を公開するインターフェイス
    /// </summary>
    public class LdapUserStore : IUserStore<LdapUser>
    {
        /// <summary>
        /// 新しいユーザーを挿入する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task CreateAsync(LdapUser ldapUser)
        {
            //TODO:CreateAsync未実装
            return null;
        }

        /// <summary>
        /// ユーザーを削除する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task UpdateAsync(LdapUser ldapUser)
        {
            //TODO:UpdateAsync未実装
            return null;
        }

        /// <summary>
        /// ユーザーを更新する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task DeleteAsync(LdapUser ldapUser)
        {
            //TODO:DeleteAsync未実装
            return null;
        }

        /// <summary>
        /// ユーザーを一意名(dn)で検索する
        /// </summary>
        /// <param name="dn">LDAP上の一意名(dn)</param>
        /// <returns></returns>
        public Task<LdapUser> FindByIdAsync(string dn)
        {
            try
            {
                using (LdapContext ldapContext = new LdapContext())
                {
                    //LDAPサーバにシステム権限で接続
                    ldapContext.Connect();
                    //該当ユーザーを取得
                    var result = ldapContext.Context.GetByDN<LdapUser>(dn);
                    if (result == null)
                    {
                        return null;
                    }
                    return Task.FromResult<LdapUser>(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// ユーザーをユーザーID(uid)で検索する
        /// </summary>
        /// <param name="uid">LDAP上のユーザーID(uid)</param>
        /// <returns></returns>
        public Task<LdapUser> FindByNameAsync(string uid)
        {
            try
            {
                //ユーザー情報を取得
                using (LdapContext ldapContext = new LdapContext())
                {
                    //LDAPサーバにシステム権限で接続
                    ldapContext.Connect();
                    //クエリで該当ユーザーを取得
                    var result = ldapContext.Context.Query<LdapUser>(SearchScope.Subtree)
                        .SingleOrDefault(x => x.Id == uid);
                    if (result == null)
                    {
                        return null;
                    }
                    //ユーザー情報を返す
                    return Task.FromResult<LdapUser>(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// ユーザー認証を行う
        /// </summary>
        /// <param name="uid">ユーザーID(uid)</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public async Task<LdapUser> Authenticate(string uid, string password)
        {
            try
            {
                //ユーザー情報を取得
                var ldapUser = await FindByNameAsync(uid);
                if (ldapUser == null)
                    return null;

                //認証
                try
                {
                    using (LdapContext ldapContext = new LdapContext())
                    {
                        //LDAPサーバにユーザ権限で接続
                        ldapContext.Connect(ldapUser.DistinguishedName, password);
                        var result = ldapContext.Context.Query<LdapUser>()
                                .SingleOrDefault(x => x.Id == uid);
                        if (result == null)
                        {
                            return null;
                        }
                        //ユーザー情報を返す
                        return result;
                    }
                }
                catch (System.DirectoryServices.Protocols.LdapException)
                {
                    //認証失敗(クエリ実行時にエラー)
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// アンマネージ リソースの解放およびリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
        }
    }
}