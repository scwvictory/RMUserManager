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
    public class LdapUserRoleController : ApiController
    {
        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("GetRoles")]
        public async Task<HttpResponseMessage> GetRoles([FromBody]LdapUser ldapUser)
        {
            var results = await new LdapUserStore().GetRolesAsync(ldapUser);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("IsInRole")]
        public async Task<HttpResponseMessage> IsInRole([FromBody]IsInRoleModel model)
        {
            var result = await new LdapUserStore().IsInRoleAsync(model.member, model.roleName);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public class IsInRoleModel
        {
            /// <summary>
            /// ロール名
            /// </summary>
            public string roleName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public LdapUser member { get; set; }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("AddToRole")]
        public async Task<HttpResponseMessage> AddToRole([FromBody]AddToRoleModel model)
        {
            await new LdapUserStore().AddToRoleAsync(model.member, model.roleName);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public class AddToRoleModel
        {
            /// <summary>
            /// ロール名
            /// </summary>
            public string roleName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public LdapUser member { get; set; }
        }

        [AuthorizeEx(Roles = "Administrator")]
        [HttpPost]
        [ActionName("RemoveFromRole")]
        public async Task<HttpResponseMessage> RemoveFromRole([FromBody]RemoveFromRoleModel model)
        {
            await new LdapUserStore().RemoveFromRoleAsync(model.member, model.roleName);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public class RemoveFromRoleModel
        {
            /// <summary>
            /// ロール名
            /// </summary>
            public string roleName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public LdapUser member { get; set; }
        }
    }
}
