using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace RMUserApi.Models
{
    /// <summary>
    /// ロール
    /// </summary>
    public class LdapUserRole
    {
        /// <summary>
        /// ロールID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 所属するメンバー
        /// </summary>
        public virtual ICollection<LdapUserRoleMember> Members { get; set; }
    }
}