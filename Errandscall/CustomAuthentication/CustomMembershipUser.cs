using System;
using ErrandscallDatabase;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Errandscall.Data;
using Errandscall.Models;

namespace Errandscall.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Roles { get; set; }

        #endregion

        public CustomMembershipUser(Login user) : base("CustomMembership", user.Client.Email, user.Id, user.Client.Email, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            UserId = user.Id;
            //FirstName = user.EmailAddress;
            //LastName = user.Surname;
            Roles = user.UserRoleId;
            

        }
    }
}