using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RazorPad.Web.Services;

namespace RazorPad.Web
{
    public class User
    {
        public long Id { get; private set; }

        public ICollection<Credential> Credentials { get; set; }

        public DateTime DateCreated { get; set; }

        public string EmailAddress { get; set; }

        public string Username { get; set; }


        public User()
        {
            Credentials = new Collection<Credential>();
        }


        public TCredential GetCredential<TCredential>(Func<TCredential, bool> predicate = null) where TCredential : Credential
        {
            return GetCredentials(predicate).SingleOrDefault();
        }

        public IEnumerable<TCredential> GetCredentials<TCredential>(Func<TCredential, bool> predicate = null) where TCredential : Credential
        {
            var credentials = Credentials.OfType<TCredential>();

            if (predicate != null)
                return credentials.Where(predicate);

            return credentials;
        }
    }


    public static class UserRespositoryExtensions
    {
        public static User FindUserByCredential<TCredential>(this IRepository repository, Func<TCredential, bool> predicate = null)
            where TCredential : Credential
        {
            var user = repository.SingleOrDefault<User>(u => u.GetCredentials(predicate).Any());

            return user;
        }

        public static User FindUserByEmail(this IRepository repository, string emailAddress)
        {
            return repository.SingleOrDefault<User>(user => user.EmailAddress == emailAddress);
        }

        public static User FindUserByUsername(this IRepository repository, string username)
        {
            return repository.SingleOrDefault<User>(user => user.Username == username);
        }
    }
}