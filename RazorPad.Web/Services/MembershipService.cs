using System;
using System.Linq;
using RazorPad.Web.Authentication;

namespace RazorPad.Web.Services
{
    public interface IMembershipService
    {
        void CreateUser(User user);
        
        string GeneratePasswordResetToken(string emailAddress, out User user);

        void ResetPassword(string username, string password, string token);
        
        bool ValidateNewUsername(string username);

        bool ValidatePasswordResetToken(string token, out User user);

        bool ValidateUser(string username, string password);
        bool ValidateUser(string username, string password, out User user);
    }


    public class MembershipService : IMembershipService
    {
        private readonly IRepository _repository;


        public MembershipService(IRepository repository)
        {
            _repository = repository;
        }


        public void CreateUser(User user)
        {
            _repository.Save(user);
            _repository.SaveChanges();
        }

        public string GeneratePasswordResetToken(string emailAddress, out User user)
        {
            var credential = GetFormsAuthCredential(u => u.EmailAddress == emailAddress, out user);

            if (credential == null) return null;

            credential.SetForgotPasswordToken();
            _repository.SaveChanges();

            return credential.ForgotPasswordToken;
        }


        public void ResetPassword(string username, string password, string token)
        {
            User user;
            var credential = GetFormsAuthCredential(u => u.Username == username, out user);

            if (credential == null)
                return;

            if (credential.ForgotPasswordToken != token)
                throw new ApplicationException("Invalid forgot password token");

            credential.SetPassword(password);
            _repository.SaveChanges();
        }

        public bool ValidateNewUsername(string username)
        {
            var existingUser = _repository.SingleOrDefault<User>(u => u.Username == username);
            return existingUser == null;
        }

        public bool ValidatePasswordResetToken(string token, out User user)
        {
            user = _repository.FindUserByCredential<FormsAuthCredential>(
                        x => x.ForgotPasswordToken == token);

            return user != null;
        }

        public bool ValidateUser(string username, string password)
        {
            User user;
            return ValidateUser(username, password, out user);
        }

        public bool ValidateUser(string username, string password, out User user)
        {
            var passwordHash = FormsAuthCredential.Create(password).Hash;

            user = _repository.FindUserByCredential<FormsAuthCredential>(
                        credential => credential.Hash == passwordHash);

            return user != null;
        }


        private FormsAuthCredential GetFormsAuthCredential(Func<User, bool> predicate, out User user)
        {
            user = _repository.SingleOrDefault(predicate);

            if (user == null)
                return null;

            var credential = user.Credentials.OfType<FormsAuthCredential>().SingleOrDefault();

            return credential;
        }
    }
}
