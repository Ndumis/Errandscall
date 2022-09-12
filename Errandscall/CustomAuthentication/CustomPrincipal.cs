using Errandscall.Data;
using Errandscall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Errandscall.CustomAuthentication
{
    public class CustomPrincipal : IPrincipal
    {
        #region Identity Properties

        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
     
        #endregion

        public IIdentity Identity
        {
            get; private set;
        }

        public bool IsInRole(string role)
        {
            return true;//TODO: fix // (Roles.Any(r => role.Split(',').Any(x => x == r)));
        }

        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }
    }
}