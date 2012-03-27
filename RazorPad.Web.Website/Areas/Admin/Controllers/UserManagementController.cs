﻿using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;
using RazorPad.Web.Services;

namespace RazorPad.Web.Website.Areas.Admin.Controllers
{
    [Authorize(Users = "jchadwick,shankars,mrchief")]
    public class UserManagementController : Controller
    {
        private readonly IRepository _repository;
        private readonly IDocumentSession _session;

        public UserManagementController(IRepository repository, IDocumentSession session)
        {
            _repository = repository;
            _session = session;
        }

        public ActionResult Index()
        {
            var users = _repository.Query<User>();
            return View("Users", users);
        }

        [NonAction]
        public ActionResult DeleteAll()
        {
            var userIds = _repository.Query<User>().AsProjection<UserId>();
            foreach (var userId in userIds)
            {
                _session.Advanced.DatabaseCommands.Delete("users-" + userId.Id, null);
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        public ActionResult Delete(long id)
        {
            _session.Advanced.DatabaseCommands.Delete("users-"+id, null);

            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(204);

            TempData.Add("Message", "Deleted user "+id);

            return RedirectToAction("Index");
        }

        public class UserId
        {
            public long Id { get; set; }
        }
    }
}