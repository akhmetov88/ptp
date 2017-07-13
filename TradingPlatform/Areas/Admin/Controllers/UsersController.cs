using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using reCAPTCHA.MVC;
using TradingPlatform.Controllers;
using TradingPlatform.Data;
using TradingPlatform.Data.DataLayer;
using TradingPlatform.Enums;
using TradingPlatform.Messaging;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [System.Web.Mvc.Authorize]
    public class UsersController : BaseController
    {
        #region var
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;
        private PtpRolesManager _rolesManager;
        private ApplicationDbContext _db;
        //    TeleAuth telegrams = new TeleAuth();

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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        public PtpRolesManager RolesManager
        {
            get { return _rolesManager ?? HttpContext.GetOwinContext().Get<PtpRolesManager>(); }
            private set { _rolesManager = value; }
        }

        private ContragentDataLayer _dl;


        public ContragentDataLayer dl
        {
            get
            {
                return _dl ?? new ContragentDataLayer(db);
            }
            private set
            {
                _dl = value;
            }
        }
        #endregion

        #region constructor
        public UsersController()
        {
            
        }
        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, PtpRolesManager rolesManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RolesManager = rolesManager;
        }
        #endregion
      


        #region заявки на аккредитацию
        [System.Web.Mvc.Authorize(Roles = "root, admin"), InLeftMenu]
        public ActionResult Index()
        {
            try
            {
                //string[] stuffroles = new string[]{"root","admin","lawyer"};

                var users = UserManager.Users.ToList().Select(
                    f => new StuffUserViewModel() {Email = f.Email, Roles = f.Roles.ToList().Select(t => GetRoleName(t.RoleId)).ToList()}).ToList();
              
                return View(users);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return View("Error");
            }
        }

        private string GetRoleName(string id)
        {
            return RolesManager.FindById(id).Name??"";
        }
        

        [System.Web.Mvc.Authorize(Roles = "root")]
        public ActionResult Create()
        {
            string[] stuffroles = new string[] { "root", "admin", "lawyer" };
            ViewBag.Role = new SelectList(RolesManager.Roles.ToList().Where(c=>stuffroles.Contains(c.Name)), "Name", "Name");
            string detectedIp = Request.UserHostAddress;
            return View(new RegisterViewModel(){Password = "PtpKz93223e4!",ConfirmPassword = "PtpKz93223e4!",DetectedIp = detectedIp});
        }

        [Authorize(Roles = "root")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    RegisterName = model.Name,
                    Registered = DateTime.UtcNow,
                    IsAcceptedOffert = true
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    try
                    {
                        var userinfo = new UserInfo()
                        {
                            User = user,
                            Name = model.Name ?? model.Email,
                            AdditionalMail = "",
                            AdditionalPhone = "",
                            Patronymyc = "",
                            Surname = ""
                        };
                        db.Insert(userinfo, User.Identity.GetUserId());
                        //  var b = RolesManager.Roles.ToList();
                        await UserManager.AddToRoleAsync(user.Id, model.Role);
                        user.Registered = DateTime.UtcNow;
                        user.AllowPromoEmails = true;
                        user.AllowTradeEmails = true;
                        user.EmailConfirmed = true;
                        user.IpRestricted = true;
                        user.AllowedIp = model.Ip;
                        user.IsDebug = true;
                        await UserManager.UpdateAsync(user);
                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code },
                            protocol: Request.Url.Scheme);
                        user.IsAcceptedOffert = true;
                        UserManager.Update(user);
                        db.SaveChanges();
                        //Mailer.SendMail(user.Email, "Для подтверждения е-мэйла перейдите по <a href=\"" + callbackUrl + "\">ccылке</a>");
                        //  Mailer.SendRegistrationMail(user.RegisterName ?? model.Name, user.Email, callbackUrl);
                        EmailFactory.SendEmailAsync(new ForgotPassword(user, callbackUrl));
                        //await m.SendMessageAsync();
                        //await EmailFactory.SendEmailAsync(new Email(m));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        UserManager.Delete(user);
                        ModelState.AddModelError("other", ex.Message);
                        return View("Error", ModelState);
                    }

                    return RedirectToAction("Index", "Users", new { name = model.Name });
                }
                else
                {
                    //ModelState.AddModelError("Other", result.Errors.First());
                    AddErrors(result);
                    logger.Error(String.Join(",", result.Errors));
                }
            }
            return View(model);
        }
        


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                EmailFactory.SendEmailAsync(new ForgotPassword(user, callbackUrl));
                // await m.SendMessageAsync();

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {

            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_rolesManager != null)
                {
                    _rolesManager.Dispose();
                    _rolesManager = null;
                }

                //if (_db != null)
                //{
                //    _db.Dispose();
                //    _db = null;
                //}
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalCookie);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public ContragentDataLayer Dl
        {
            get
            {
                return dl;
            }

            set
            {
                dl = value;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }



        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}