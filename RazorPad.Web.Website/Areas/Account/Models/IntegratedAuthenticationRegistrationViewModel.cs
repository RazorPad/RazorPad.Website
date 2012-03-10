namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class IntegratedAuthenticationRegistrationViewModel
    {
        public IntegratedAuthenticationCredentialsRequest Credentials { get; set; }
        public CreateNewUserRequest Request { get; set; }

        public IntegratedAuthenticationRegistrationViewModel()
        {
            Credentials = new IntegratedAuthenticationCredentialsRequest();
            Request = new CreateNewUserRequest();
        }

        public IntegratedAuthenticationRegistrationViewModel(CreateNewUserRequest request, IntegratedAuthenticationCredentialsRequest credentials)
        {
            Request = request;
            Credentials = credentials;
        }
    }
}