using System;

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
            user = _repository.SingleOrDefault<User>(u => u.EmailAddress == emailAddress);

            if (user == null)
                return null;

            user.ForgotPasswordToken = Guid.NewGuid().ToString("N");
            _repository.SaveChanges();
            
            return user.ForgotPasswordToken;
        }


        public void ResetPassword(string username, string password, string token)
        {
            var user = _repository.SingleOrDefault<User>(u => u.Username == username && u.ForgotPasswordToken == token);

            if (user == null) 
                return;

            user.Password = password;
            _repository.SaveChanges();
        }

        public bool ValidateNewUsername(string username)
        {
            var existingUser = _repository.SingleOrDefault<User>(u => u.Username == username);
            return existingUser == null;
        }

        public bool ValidatePasswordResetToken(string token, out User user)
        {
            user = _repository.SingleOrDefault<User>(u => u.ForgotPasswordToken == token);
            return user != null;
        }

        public bool ValidateUser(string username, string password)
        {
            User user;
            return ValidateUser(username, password, out user);
        }

        public bool ValidateUser(string username, string password, out User user)
        {
            user = _repository.SingleOrDefault<User>(u => u.Username == username && u.Password == password);
            return user != null;
        }
    }
}
