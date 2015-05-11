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
    /// OU管理 API を公開するインターフェイス
    /// </summary>
    public class LdapOuStore : IDisposable
    {
        /// <summary>
        /// OUの一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<LdapOu>> FindAsync()
        {
            try
            {
                using (LdapContext ldapContext = new LdapContext())
                {
                    //LDAPサーバにシステム権限で接続
                    ldapContext.Connect();
                    //該当ユーザーを取得
                    var results = ldapContext.Context.Query<LdapOu>().ToList();
                    return Task.FromResult<List<LdapOu>>(results);
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