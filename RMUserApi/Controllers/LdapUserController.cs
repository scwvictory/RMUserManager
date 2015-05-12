using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading.Tasks;
using RMUserApi.Attributes;
using RMUserApi.Ldap;
using RMUserApi.Models;
using RMUserApi.Utilities;

namespace RMUserApi.Controllers
{
    public class LdapUserController : ApiController
    {
        /// <summary>
        /// 現在ログイン中のユーザー情報を取得する
        /// </summary>
        /// <returns></returns>
        [AuthorizeEx]
        [HttpPost]
        [ActionName("GetCurrentUser")]
        public async Task<HttpResponseMessage> GetCurrentUser()
        {
            try
            {
                //クレーム情報から一意名を取得
                var dn = Request.GetOwinContext().Authentication.User.Claims
                    .Single(x => x.Type == "dn").Value;
                //一意名をキーにしてユーザー情報を取得
                var result = await new LdapUserStore().FindByIdAsync(dn);
                //ユーザー情報を返す
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("SearchUsers")]
        public async Task<HttpResponseMessage> SearchUsers([FromBody]LdapUserStore.SearchAsyncModel model)
        {
            try
            {
                //ユーザーIDをキーにしてユーザー情報を取得
                var results = await new LdapUserStore().SearchAsync(model);
                //ユーザー情報を返す
                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }        

        /// <summary>
        /// 指定されたユーザーIDに該当するユーザー情報を取得する
        /// </summary>
        /// <param name="uid">ユーザーID(uid)</param>
        /// <returns></returns>
        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("GetUser")]
        public async Task<HttpResponseMessage> GetUser([FromBody]LdapUser ldapUser)
        {
            try
            {
                //ユーザーIDをキーにしてユーザー情報を取得
                var result = await new LdapUserStore().FindByNameAsync(ldapUser.Id);
                //ユーザー情報を返す
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("CreateUser")]
        public async Task<HttpResponseMessage> CreateUser([FromBody]LdapUser ldapUser)
        {
            try
            {
                //ユーザーを追加
                await new LdapUserStore().CreateAsync(ldapUser);
                //登録した結果を返す
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("UpdateUser")]
        public async Task<HttpResponseMessage> UpdateUser([FromBody]LdapUser ldapUser)
        {
            try
            {
                //ユーザーを更新
                await new LdapUserStore().UpdateAsync(ldapUser);
                //更新した結果を返す
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("DeleteUser")]
        public async Task<HttpResponseMessage> DeleteUser([FromBody]LdapUser ldapUser)
        {
            try
            {
                //ユーザーを削除
                await new LdapUserStore().DeleteAsync(ldapUser);
                //更新した結果を返す
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("SetPassword")]
        public async Task<HttpResponseMessage> SetPassword([FromBody]SetPasswordModel model)
        {
            try
            {
                //該当ユーザーを取得
                var ldapUser = await new LdapUserStore().FindByNameAsync(model.Id);

                //パスワードをハッシュ化
                var passwordHash = SSHASaltedGenerator.GenerateSaltedSHA1(model.Password);

                //パスワードを更新
                await new LdapUserStore().SetPasswordHashAsync(ldapUser, passwordHash);              
                
                //更新した結果を返す
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        public class SetPasswordModel
        {
            /// <summary>
            /// ユーザーID(uid)
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// パスワード(プレーンテキスト)
            /// </summary>
            public string Password { get; set; }
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public async Task<HttpResponseMessage> ChangePassword([FromBody]ChangePasswordModel model)
        {
            try
            {
                //ユーザーID、パスワードによる認証
                var ldapUser = await new LdapUserStore().Authenticate(model.Id, model.OldPassword);
                if (ldapUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                //パスワードをハッシュ化
                var passwordHash = SSHASaltedGenerator.GenerateSaltedSHA1(model.NewPassword);

                //パスワードを更新
                await new LdapUserStore().SetPasswordHashAsync(ldapUser, passwordHash);

                //更新した結果を返す
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        public class ChangePasswordModel
        {
            /// <summary>
            /// ユーザーID(uid)
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 旧パスワード
            /// </summary>
            public string OldPassword { get; set; }
            /// <summary>
            /// 新パスワード
            /// </summary>
            public string NewPassword { get; set; }
        }
    }
}
