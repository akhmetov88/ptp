using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TradingPlatform.Models.ViewModel;
using TradingPlatform.Models;

namespace TradingPlatform.Controllers
{
    public class ProfileController : BaseController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationDbContext _db;

        public ApplicationDbContext db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }
      //  ProfileDataLayer dl = new ProfileDataLayer();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            return View(user.InboxNotifications.Select(x => new NotificationViewModel()
            {
                Id = x.Id,
                IsViewed = x.IsViewed,
                Subject = x.Subject,
                Body = x.Body,
                FromUserId = x.FromUserId,
                FromUserName = x.FromUser.UserName,
                ToUserId = x.ToUserId,
                ToUserName = x.ToUser.UserName,
                CreateDate = x.CreateDate,
                ViewedDate = x.ViewedDate
            }));
        }
    }
}