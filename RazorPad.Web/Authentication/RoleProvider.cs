using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace RazorPad.Web.Authentication
{
    [CollectionDataContractAttribute(Namespace = "")]
    public class Roles : List<Role>
    {
        public Role FindByName(string name)
        {
            return this.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<Role> FindByUsername(string username)
        {
            return this.Where(x => x.Users.Contains(username));
        }
    }

    [DataContract(Namespace = "")]
    public class Role
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<string> Users { get; set; }

        public Role()
        {
            Users = new List<string>();
        }

        public Role(string name)
        {
            Name = name;
        }
    }

    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public override string ApplicationName { get; set; }

        public string ConfigFile { get; set; }

        protected string ConfigFilePath
        {
            get { return System.Web.HttpContext.Current.Server.MapPath(ConfigFile); }
        }

        protected Roles Roles
        {
            get
            {
                if(_roles != null)
                    return _roles;

                try
                {
                    using (var stream = File.OpenRead(ConfigFilePath))
                        return _roles = (Roles)new DataContractSerializer(typeof(Roles)).ReadObject(stream);
                }
                catch (Exception)
                {
                    return new Roles();
                }
            }
        }
        private Roles _roles;

        public RoleProvider()
        {
            ConfigFile = @"~\App_Data\Roles.xml";
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = Roles.FindByName(roleName);

                if (role == null)
                    continue;

                role.Users.AddRange(usernames);
            }

            Save();
        }

        public override void CreateRole(string roleName)
        {
            var role = Roles.FindByName(roleName);
            
            if (role != null)
                return;

            Roles.Add(new Role(roleName));

            Save();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = Roles.FindByName(roleName);

            if (role == null)
                return false;
            
            if (throwOnPopulatedRole && role.Users.Any())
                throw new ApplicationException("Tried to delete role but it has users assigned!");

            Roles.Remove(role);

            Save();

            return true;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetRolesForUser(username).Contains(roleName);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName).Where(x => x == usernameToMatch).ToArray();
        }

        public override string[] GetAllRoles()
        {
            return Roles.Select(x => x.Name).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return Roles.FindByUsername(username).Select(x => x.Name).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return Roles.Where(x => x.Name == roleName)
                .SelectMany(x => x.Users)
                .ToArray();
        }

        public override bool RoleExists(string roleName)
        {
            return Roles.FindByName(roleName) != null;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = Roles.FindByName(roleName);

                if (role == null)
                    continue;

                role.Users.RemoveAll(usernames.Contains);
            }

            Save();
        }

        private void Save()
        {
            using(var stream = File.OpenWrite(ConfigFilePath))
                new DataContractSerializer(typeof(Roles)).WriteObject(stream, Roles);
        }
    }
}
