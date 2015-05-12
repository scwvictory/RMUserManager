using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMUserApi.Ldap
{
    /// <summary>
    /// 指定されたユーザーが見つからない場合
    /// </summary>
    public class NotFoundLdapUserException : ApplicationException
    {
        public NotFoundLdapUserException()
        {
        }

        public NotFoundLdapUserException(string message)
            : base(message)
        {
        }

        public NotFoundLdapUserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 同じIDのユーザーが存在する場合
    /// </summary>
    public class DuplicateLdapUserIdException: ApplicationException
    {
        public DuplicateLdapUserIdException()
        {
        }

        public DuplicateLdapUserIdException(string message)
            : base(message)
        {
        }

        public DuplicateLdapUserIdException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 指定されたロールが存在しない場合
    /// </summary>
    public class NotFoundLdapUserRoleException : ApplicationException
    {
        public NotFoundLdapUserRoleException()
        {
        }

        public NotFoundLdapUserRoleException(string message)
            : base(message)
        {
        }

        public NotFoundLdapUserRoleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}