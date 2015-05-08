using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using RMUserApi.Ldap;

namespace RMUserApi.Providers
{
    public class LdapAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                //認証処理
                //LDAPに対して uid + password で認証を行う
                var userManager = new LdapUserManager(new LdapUserStore());
                var ldapUser = await userManager.FindAsync(context.UserName, context.Password);
                if (ldapUser == null)
                {
                    //認証失敗
                    context.SetError("invalid_grant", "The username or password is incorrect.");
                    return;
                }

                //ユーザーを表す ClaimsIdentity を作成する
                var identity = await userManager.CreateIdentityAsync(ldapUser, context.Options.AuthenticationType);
                identity.AddClaim(new Claim("role", "SystemAdmin"));
                identity.AddClaim(new Claim("dn", ldapUser.DistinguishedName));
                context.Validated(identity);

                //認証登録
                context.Request.Context.Authentication.SignIn(identity);
            }
            catch (Exception e)
            {
                context.SetError("Application Error", e.Message);
            }
        }
    }
}