using System.Web.Mvc;
using RazorPad.Web.Authentication;
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
        public ActionResult Register()
        {
            return View("Register", new CreateNewUserRequest());
        }

        [HttpPost]
        public ActionResult Register(CreateNewUserRequest createNewUserRequest)
        {
            var isValidUsername = _membershipService.ValidateNewUsername(createNewUserRequest.Username);

            if (isValidUsername == false)
                ModelState.AddModelError("username", "Invalid user name (user already exists?)");

            if (ModelState.IsValid == false)
            {
                createNewUserRequest.Password = null;
                createNewUserRequest.PasswordConfirm = null;
                return View("Register", createNewUserRequest);
            }

            var user = new User
            {
                Username = createNewUserRequest.Username,
                EmailAddress = createNewUserRequest.Email,
            };

            user.Credentials.Add(FormsAuthCredential.Create(createNewUserRequest.Password));

            _membershipService.CreateUser(user);

            FormsAuthController.AuthenticateUserThunk(createNewUserRequest.Username);

            return Redirect(createNewUserRequest.RedirectUrl);
        }
    }
}
