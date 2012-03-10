using System.Web.Mvc;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Areas.Account.Models;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IMembershipService _membershipService;


        public RegistrationController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }


        [HttpGet]
        public ActionResult Integrated(
            [Bind(Prefix = "")]CreateNewUserRequest createNewUserRequest,
            [Bind(Prefix = "")]IntegratedAuthenticationCredentialsRequest credentials)
        {
            return View("Integrated", 
                        new IntegratedAuthenticationRegistrationViewModel(createNewUserRequest, credentials));
        }


        [HttpPost, ActionName("Integrated")]
        public ActionResult IntegratedPost(
            [Bind(Prefix = "")]CreateNewUserRequest createNewUserRequest,
            [Bind(Prefix = "")]IntegratedAuthenticationCredentialsRequest credentials)
        {
            ValidateUsername(createNewUserRequest);

            if (ModelState.IsValid == false)
            {
                return View("Integrated",
                            new IntegratedAuthenticationRegistrationViewModel(createNewUserRequest, credentials));
            }

            return CreateNewUser(createNewUserRequest, credentials);
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View("Register", new CreateNewUserRequest());
        }

        [HttpPost]
        public ActionResult Register(
            [Bind(Prefix = "")]CreateNewUserRequest createNewUserRequest, 
            [Bind(Prefix = "")]PasswordRequest credentials)
        {
            ValidateUsername(createNewUserRequest);

            if (ModelState.IsValid == false)
            {
                return View("Register", createNewUserRequest);
            }

            return CreateNewUser(createNewUserRequest, credentials);
        }


        private void ValidateUsername(CreateNewUserRequest createNewUserRequest)
        {
            var isValidUsername = _membershipService.ValidateNewUsername(createNewUserRequest.Username);

            if (isValidUsername == false)
                ModelState.AddModelError("username", "Invalid user name (user already exists?)");
        }

        private ActionResult CreateNewUser(CreateNewUserRequest createNewUserRequest, CredentialRequest credentials)
        {
            var user = new User
                           {
                               Username = createNewUserRequest.Username,
                               EmailAddress = createNewUserRequest.EmailAddress,
                           };

            user.Credentials.Add(credentials.ToCredential());

            _membershipService.CreateUser(user);

            FormsAuthController.AuthenticateUserThunk(createNewUserRequest.Username);

            return Redirect(createNewUserRequest.RedirectUrl);
        }
    }
}
