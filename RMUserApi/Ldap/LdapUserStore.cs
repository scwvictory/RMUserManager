using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using System.DirectoryServices.Protocols;
using Microsoft.AspNet.Identity;
using RMUserApi.Models;
using RMUserApi.Utilities;

namespace RMUserApi.Ldap
{
    /// <summary>
    /// ユーザー管理 API を公開するインターフェイス
    /// </summary>
    public class LdapUserStore : IUserStore<LdapUser>, IUserPasswordStore<LdapUser>
    {
        /// <summary>
        /// 新しいユーザーを挿入する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task CreateAsync(LdapUser ldapUser)
        {
            try
            {
                //ユーザーIDの重複チェック
                try
                {
                    var duplicateUser = FindByNameAsync(ldapUser.Id);
                    //同じuidが見つかった場合はNG
                    if (duplicateUser != null)
                    {
                        throw new DuplicateLdapUserIdException(
                            string.Format("uid: '{0}' is already exists.", ldapUser.Id));
                    }
                }
                catch (NotFoundLdapUserException)
                {
                    //同じuidが見つからなければOK
                }

                using (LdapContext ldapContext = new LdapContext())
                {
                    ldapContext.Connect();

                    //ユーザーの一意名(dn)、表示名を設定
                    ldapUser.SetDistinguishedName();
                    ldapUser.SetDisplayName();
                    //追加処理
                    ldapContext.Context.Add(ldapUser);
                    return Task.FromResult<LdapUser>(ldapUser);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// ユーザーを更新する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task UpdateAsync(LdapUser ldapUser)
        {
            try
            {
                using (LdapContext ldapContext = new LdapContext())
                {
                    ldapContext.Connect();

                    //LDAPから当該ユーザを取得
                    var result = FindByNameAsync(ldapUser.Id).Result;

                    //OUが異なる場合
                    if (ldapUser.Ou != result.Ou)
                    {
                        //オブジェクトを移動
                        ldapUser.SetDistinguishedName();
                        var fromDn = result.DistinguishedName;
                        var toDn = string.Format("{0}{1}", ldapUser.Ou == null ? "" : "ou=" + ldapUser.Ou + ",", LdapConfig.NamingContext);
                        ldapContext.Context.MoveEntry(fromDn, toDn);
                        //移動後、再取得
                        result = FindByNameAsync(ldapUser.Id).Result;
                    }

                    //渡された内容に更新
                    _CopyLdapUserProperties(ref ldapUser, ref result);
                    ldapContext.Context.Update(result);
                    return Task.FromResult<LdapUser>(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// LdapUserクラス同士でプロパティをコピーする
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void _CopyLdapUserProperties(ref LdapUser source, ref LdapUser target)
        {
            target.Id = source.Id;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.MailAddress = source.MailAddress;
            target.OrganizationName = source.OrganizationName;
            target.Ou = source.Ou;
            target.SetDistinguishedName();
            target.SetDisplayName();
        }

        /// <summary>
        /// ユーザーを削除する
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <returns></returns>
        public Task DeleteAsync(LdapUser ldapUser)
        {
            try
            {
                using (LdapContext ldapContext = new LdapContext())
                {
                    ldapContext.Connect();

                    //LDAPから当該ユーザを取得
                    var result = FindByNameAsync(ldapUser.Id).Result;

                    //渡された内容を削除
                    ldapContext.Context.Delete(result.DistinguishedName);
                    return Task.FromResult<LdapUser>(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
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
                        throw new NotFoundLdapUserException(
                            string.Format("dn: '{0}' is not found into LDAP server.", dn));
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
                        throw new NotFoundLdapUserException(
                            string.Format("uid: '{0}' is not found into LDAP server.", uid));
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

                //認証
                try
                {
                    using (LdapContext ldapContext = new LdapContext())
                    {
                        //LDAPサーバにユーザ権限で接続
                        ldapContext.Connect(ldapUser.DistinguishedName, password);
                        var result = ldapContext.Context.Query<LdapUser>()
                                .SingleOrDefault(x => x.Id == uid);
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

        public Task<string> GetPasswordHashAsync(LdapUser ldapUser)
        {
            //実装なし
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(LdapUser ldapUser)
        {
            //実装なし
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(LdapUser ldapUser, string passwordHash)
        {
            try
            {
                using (LdapContext ldapContext = new LdapContext())
                {
                    //LDAPサーバーへ接続
                    ldapContext.Connect();

                    //パスワードを設定するユーザー情報を取得
                    var ldapUserPwd = ldapContext.Context.Query<LdapUserPassword>()
                        .SingleOrDefault(x => x.Id == ldapUser.Id);
                    if (ldapUserPwd == null)
                    {
                        //該当ユーザーなし
                        throw new NotFoundLdapUserException(
                            string.Format("uid: '{0}' is not found.", ldapUser.Id));
                    }

                    //ハッシュ化済みパスワードを設定する
                    ldapUserPwd.Password = passwordHash;
                    ldapContext.Context.Update<LdapUserPassword>(ldapUserPwd);
                    return Task.FromResult<LdapUserPassword>(ldapUserPwd);
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