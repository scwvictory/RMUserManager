using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading.Tasks;
using RMUserApi.Ldap;

namespace RMUserApi.Controllers
{
    public class LdapUserController : ApiController
    {
        /// <summary>
        /// 現在ログイン中のユーザー情報を取得する
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUser")]
        public Task<HttpResponseMessage> GetUser()
        {
            //クレーム情報から一意名を取得
            var dn = Request.GetOwinContext().Authentication.User.Claims
                .Single(x => x.Type == "dn").Value;
            //一意名をキーにしてユーザー情報を取得
            var ldapUser = new LdapUserStore().FindByIdAsync(dn);
            //ユーザー情報を返す
            return Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, ldapUser));
        }

        /// <summary>
        /// 指定されたユーザーIDに該当するユーザー情報を取得する
        /// </summary>
        /// <param name="uid">ユーザーID(uid)</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("GetUser")]
        public Task<HttpResponseMessage> GetUser(string uid)
        {
            //ユーザーIDをキーにしてユーザー情報を取得
            var ldapUser = new LdapUserStore().FindByNameAsync(uid);
            //ユーザー情報を返す
            return Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, ldapUser));
        }
    }
}
