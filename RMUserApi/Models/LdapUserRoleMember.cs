using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace RMUserApi.Models
{
    /// <summary>
    /// ロールメンバー
    /// </summary>
    public class LdapUserRoleMember
    {
        /// <summary>
        /// ロールメンバーID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// LDAP上のユーザーID(uid)
        /// </summary>
        [Required, StringLength(200)]
        public string LdapId { get; set; }

        /// <summary>
        /// 所属するロール
        /// </summary>
        public virtual ICollection<LdapUserRole> LdapUserRoles { get; set; }
    }
}