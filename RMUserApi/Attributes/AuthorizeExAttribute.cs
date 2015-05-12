using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RMUserApi.Ldap;
using RMUserApi.Models;

namespace RMUserApi.Attributes
{
    public class AuthorizeExAttribute : AuthorizeAttribute
    {
        public new string Roles { get; set; }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //トークン認証(既定の認証)
            base.Roles = null;
            if (!base.IsAuthorized(actionContext))
            {
                return false;
            }

            //権限取得
            if (this.Roles != null)
            {
                var uid = ((HttpRequestMessage)actionContext.Request).GetOwinContext().Authentication.User.Claims
                    .Single(x => x.Type == "uid").Value;
                using (RMUserDbContext context = new RMUserDbContext())
                {
                    var roles = context.LdapUserRoleMembers
                        .Where(m => m.LdapId == uid)
                        .SelectMany(m => m.LdapUserRoles.Select(r => r.Name))
                        .ToList();
                    foreach (var r in this.Roles.Split(','))
                    {
                        if (roles.Contains(r))
                        {
                            return true;    //該当するロールがあった場合
                        }
                    }
                    return false;           //該当するロールを持っていなかった場合
                }
            }
            return true;                    //ロールの指定がなければ無条件で許可
        }
    }
}