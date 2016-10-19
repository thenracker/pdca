using System.Web.Security;
using DataAccess.Models.DataUnit.Users;

namespace Web.Class
{
    public class ImplRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            if (roleName == "authorized")
                return Uzivatel.UserExists(username);

            var user = new Uzivatel(username);
            return user.Role.Identificator == roleName;
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = new Uzivatel(username);
            return user.Role == null ? new[] { Uzivatel.UserExists(username) ? "authorized" : "" } : new string[] {user.Role.Identificator, "authorized" };
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}