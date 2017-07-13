using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using Rotativa;
using TradingPlatform.Data;
using TradingPlatform.Messaging;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Models.PageModel;
using TradingPlatform.Models.ViewModel;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using TradingPlatform.Areas.Admin.Models;

//using TradingPlatform.Data;

namespace TradingPlatform.Controllers
{
    public class HomeController : Controller
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



        private static Logger logger = LogManager.GetCurrentClassLogger();
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
        ContentDataLayer contentDataLayer = new ContentDataLayer();
        public ActionResult CreateContentsToSeedMethod()
        {

            var models =
                db.ContentPages.Select(
                    c =>
                        new ContentView()
                        {
                            Alias = c.Alias,
                            Descriptions = c.Descriptions,
                            HtmlContent = c.HtmlContent,
                            KeyWords = c.KeyWords,
                            Lang = c.Lang,
                            Title = c.Title
                        }).ToList();
            return View(models);

        }
      

        [HttpGet]
        public async Task Tele()
        {
            try
            {
                if (TradingPlatform.Messaging.Telegram.client.IsUserAuthorized())
                    await TradingPlatform.Messaging.Telegram.Broadcast(new Broadcast() { Body = "Body", Subject = "Извинитье, надо потестиць" }, "ptpuatest");
                else
                    await TradingPlatform.Messaging.Telegram.AuthUser();
            }
            catch (Exception ex)
            {
                 EmailFactory.SendEmail("admin@ptp.ua", "EXCEPTION", ex.Message);
            }
        }

        [HttpGet]
        public ActionResult T()
        {
            return Redirect("https://t.me/ptpua");
        }
        private void Seed(TradingPlatform.Models.ApplicationDbContext context)
        {
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Tokens] ON");
            foreach (var model in GetTokenz())
            {
                if (context.Tokens.FirstOrDefault(c => c.Id == model.Id) == null)
                    context.Tokens.Add(new Token(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Tokens] OFF");
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Contents] ON");
            foreach (var model in GetContent())
            {
                if (context.ContentPages.FirstOrDefault(c => c.Id == model.Id) == null)
                    context.ContentPages.Add(new Content(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Contents] OFF");
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Catalogs] ON");
            foreach (var model in GetCatalogs())
            {
                if (context.Catalogs.FirstOrDefault(c => c.Id == model.Id) == null)
                    context.Catalogs.Add(new Catalog(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Catalogs] OFF");
        }

        private List<TokenModel> GetTokenz()
        {
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/tokens");
                var models = JsonConvert.DeserializeObject<List<TokenModel>>(data);
                var t = models.Count;
                return models.ToList();
            }
        }
        private List<ContentView> GetContent()
        {
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/contents");
                var models = JsonConvert.DeserializeObject<IEnumerable<ContentView>>(data);
                return models.ToList();
            }
        }
        private List<CatalogModel> GetCatalogs()
        {
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/catalogs");
                var models = JsonConvert.DeserializeObject<IEnumerable<CatalogModel>>(data);
                return models.OrderBy(c => c.Id).ToList();
            }
        }




        [HttpGet]
        public ActionResult Startup()
        {
            Seed(db);
            var detectedIp = Request.UserHostAddress;
            if (UserManager.Users.Any())
            {
                return RedirectToAction("Index");
            }
            
            return View(new RegisterViewModel(){DetectedIp = detectedIp});
        }
        [HttpPost]
        public ActionResult Startup(RegisterViewModel model)
        {

            if (UserManager.Users.Any())
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var user = new Models.ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    RegisterName = model.Name,
                    Registered = DateTime.UtcNow,
                    IsAcceptedOffert = true
                };
                var result = UserManager.Create(user, model.Password);
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
                        UserManager.AddToRole(user.Id, "root");
                        user.Registered = DateTime.UtcNow;
                        user.AllowPromoEmails = true;
                        user.AllowTradeEmails = true;
                        user.EmailConfirmed = true;
                        user.AllowedIp = model.Ip;
                        user.IsDebug = true;
                        UserManager.Update(user);

                        user.IsAcceptedOffert = true;
                        UserManager.Update(user);
                        db.SaveChanges();
                        //Mailer.SendMail(user.Email, "Для подтверждения е-мэйла перейдите по <a href=\"" + callbackUrl + "\">ccылке</a>");
                        //  Mailer.SendRegistrationMail(user.RegisterName ?? model.Name, user.Email, callbackUrl);
                        EmailFactory.SendEmailAsync(new SuccessRegister(user,"")); // {Link = callbackUrl};
                        //await m.SendMessageAsync();
                        //await EmailFactory.SendEmailAsync(new Email(m));
                    }
                    catch (Exception ex)
                    {
                        return View("Error");
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult SendMails()
        {
            try
            {
                var mo = new Models.NotifyModels.Broadcast()
                {
                    Body = "Шановні покупці! Повідомляємо, що на ЕТП РТР оголошено нові торги, запрошуємо до участі!",
                    Subject = "Нові торги"
                };
                ViewBag.Message = "Отправить?";
                return View(mo);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", ex);
                return View("Error", ModelState);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SendMails(Broadcast model)
        {
            try
            {
                ViewBag.Message = "ГОТОВО";
                //await EmailFactory.Brodcast(mod);
                if (model.ToAll)
                {
                    foreach (var user in UserManager.Users.ToList())
                    {
                        EmailFactory.SendEmailAsync(model, user);
                    }
                }
                else
                {
                    EmailFactory.SendEmailAsync(model);
                    if (model.IsReply)
                    {
                        var feedback = db.Feedbacks.Find(model.ReplyId);
                        if (feedback != null)
                            feedback.IsCommited = true;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Feedback", new { area = "Admin" });
                    }
                }
                
                return View(new Broadcast()
                {
                    Body = "Шановні покупці! Повідомляємо, що на ЕТП РТР оголошено нові торги, запрошуємо до участі!",
                    Subject = "Нові торги"
                });

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", ex);
                return View("Error");
            }

        }


        #region content pages

        public ActionResult Doc(string page, bool pdf = false)
        {
            var _cnt = contentDataLayer.Get(page);
            if (_cnt != null)
            {
                if (pdf)
                {
                    return new PartialViewAsPdf(_cnt);
                }
                return View(_cnt);
            }
            return View("Error");
        }




        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult Index()
        {
            if (!UserManager.Users.Any())
            {
                return RedirectToAction("Startup");
            }
            ViewBag.MenuAnchor = "main";
            ViewBag.Title = LocalText.Inst.Get("title", "mainPageTitle", "Головна", "Главная");
            var _cnt = contentDataLayer.Get("main");
            return View(_cnt);
        }
        //  [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult AccreditationRules()
        {
            ViewBag.MenuAnchor = "accreditation";
            var _cnt = contentDataLayer.Get("accreditation");
            return View("Index", _cnt);
        }
        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult HowItUse()
        {
            ViewBag.MenuAnchor = "howituse";
            var _cnt = contentDataLayer.Get("howituse");
            return View("Index", _cnt);
        }
        //  [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult Documents()
        {
            ViewBag.MenuAnchor = "documents";
            var _cnt = contentDataLayer.Get("documents");
            return View("Index", _cnt);
        }
        //    [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult Therms()
        {
            ViewBag.MenuAnchor = "therms";
            var _cnt = contentDataLayer.Get("therms");
            return View("Index", _cnt);
        }


        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult Rules()
        {
            ViewBag.MenuAnchor = "rules";
            var _cnt = contentDataLayer.Get("rules");
            return View("Index", _cnt);
        }
        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult About()
        {
            ViewBag.MenuAnchor = "about";
            var _cnt = contentDataLayer.Get("about");
            return View("Index", _cnt);
        }
        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
        public ActionResult PrivacyPolicy()
        {
            ViewBag.MenuAnchor = "privacypolicy";
            var _cnt = contentDataLayer.Get("privacypolicy");
            return View("Index", _cnt);
        }


        public ActionResult FAQ()
        {
            ViewBag.MenuAnchor = "faq";

            return View(StaticData.Inst.GetAllHelps());

        }

        public ActionResult GetContent(string alias)
        {
            ViewBag.MenuAnchor = alias;
            var _cnt = contentDataLayer.Get(alias);
            return PartialView(_cnt);
        }
        #endregion

        public ActionResult Contacts()
        {
            ViewBag.MenuAnchor = "contacts";
            return View();
        }

        public ActionResult Feedback()
        {
            ViewBag.MenuAnchor = "feedback";
            var feedbackmodel = new FeedbackViewModel();
            if (this.Request.IsAuthenticated)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                feedbackmodel.Email = user.Email;
                feedbackmodel.Name = user.RegisterName ?? user.UserName;
                feedbackmodel.PhoneNumber = user.PhoneNumber;

            }
            return View(feedbackmodel);

        }

        [HttpPost]
        public ActionResult Feedback(FeedbackViewModel feedback)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    var feed = new Feedback()
                    {
                        Email = feedback.Email,
                        Name = feedback.Name,
                        PhoneNumber = feedback.PhoneNumber,
                        Subject = feedback.Subject,
                        Text = feedback.Text,
                        Date = DateTime.UtcNow,
                        IsCommited = false,
                    };
                    if (user != null)
                        feed.UserId = user.Id;

                    db.Insert(feed, User.Identity.GetUserId());
                    string realuser = "";
                    if (feed.UserId != null)
                    {
                        realuser = "(зарегистрирован как " + user.Email + $" ({user.RegisterName ?? user.UserName}))";
                    }

                    Mailer.SendFeedback("feedback@ptp.ua", $"ОБРАТНАЯ СВЯЗЬ: {feedback.Subject}", $"Пользователь {feedback.Name} {realuser} (Email:<a href=\"mailto: {feedback.Email}\">{feedback.Email}</a>, тел. №: {feedback.PhoneNumber}) отправил сообщение с темой <b>{feedback.Subject}</b>: <br /> <b><p>{feedback.Text}</p></b>");
                }
                else
                {
                    ViewBag.MenuAnchor = "feedback";
                    var feedbackmodel = new FeedbackViewModel();
                    if (this.Request.IsAuthenticated)
                    {
                        var user = UserManager.FindById(User.Identity.GetUserId());
                        feedbackmodel.Email = user.Email;
                        feedbackmodel.Name = user.RegisterName ?? user.UserName;
                        feedbackmodel.PhoneNumber = user.PhoneNumber;
                    }
                    return View(feedback);
                }
                return View("FeedBackOk");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("Text", ex.InnerException.Message);
                return View(feedback);
            }

        }

        public ActionResult Help(string page)
        {
            var _cnt = contentDataLayer.Get($"help-{page}");
            return PartialView("Help", _cnt.HtmlContent);
        }

        public ActionResult InfoCarusel()
        {
            return PartialView();
        }

        #region Auth
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Переход опосля регистрации
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterThanks(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                var userid = User.Identity.GetUserId();
                name = db.Users.FirstOrDefault(c => c.Id == userid)?.RegisterName;
            }
            ContentView _cnt = contentDataLayer.Get("registerthanks");
            var replace = _cnt.HtmlContent.Replace("{user}", name);
            _cnt.HtmlContent = replace;
            return View("Index", _cnt);
        }
        /// <summary>
        /// Переход опосля создания торгов
        /// </summary>
        /// <returns></returns>
        public ActionResult Success()
        {
            var userid = User.Identity.GetUserId();
            var name = db.Users.FirstOrDefault(c => c.Id == userid)?.RegisterName;

            ContentView _cnt = contentDataLayer.Get("tradethanks");
            var replace = _cnt.HtmlContent.Replace("{user}", name);
            _cnt.HtmlContent = replace;
            return View("Index", _cnt);
        }
        /// <summary>
        /// Переход опосля регистрации
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmEmail(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                var userid = User.Identity.GetUserId();
                name = db.Users.FirstOrDefault(c => c.Id == userid)?.RegisterName;
            }
            ContentView _cnt = contentDataLayer.Get("confirmemail");
            var replace = _cnt.HtmlContent.Replace("{user}", name);
            _cnt.HtmlContent = replace;
            return View("Index", _cnt);
        }
        public ActionResult RegisterThanksJuridical()
        {
            var userid = User.Identity.GetUserId();
            string name = db.Users.FirstOrDefault(c => c.Id == userid)?.RegisterName;
            ContentView _cnt = contentDataLayer.Get("registerthanksjur");
            var replace = _cnt.HtmlContent.Replace("{user}", name);
            _cnt.HtmlContent = replace;
            return View("Index", _cnt);
        }

        public ActionResult EditThanksJuridical()
        {
            var userid = User.Identity.GetUserId();
            string name = db.Users.FirstOrDefault(c => c.Id == userid)?.RegisterName;
            ContentView _cnt = contentDataLayer.Get("editthanksjur");
            var replace = _cnt.HtmlContent.Replace("{user}", name);
            _cnt.HtmlContent = replace;
            return View("Index", _cnt);
        }
        #endregion

        [HttpPost]
        public ActionResult ChangeLanguage(string lang)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            string returnUrl = Request.UrlReferrer?.PathAndQuery;
            List<string> cultures = new List<string>() { "uk-UA", "ru-RU" };
            if (!cultures.Contains(lang))
                lang = "ru-RU";

            if (user != null)
            {
                user.Locale = lang;
                UserManager.Update(user);
            }
            HttpCookie cookie = Request.Cookies.Get("language");
            if (cookie != null)
            {
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.SetCookie(cookie);
            }
            else
            {
                cookie = LocalText.CreateCookie();
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);
            }
            return Redirect(returnUrl ?? "/");
        }

        [HttpGet]
        public ActionResult Uk()
        {
            string returnUrl = Request.UrlReferrer?.PathAndQuery;
            ChangeLanguage("uk-Ua");
            return Redirect(returnUrl ?? "/");
        }
        [HttpGet]
        public ActionResult Ru()
        {
            string returnUrl = Request.UrlReferrer?.PathAndQuery;
            ChangeLanguage("ru-RU");
            return Redirect(returnUrl ?? "/");
        }
        [AllowAnonymous]
        public ActionResult Robots()
        {
            logger.Info("ROBOTS: " + Request.UserHostName + Request.UserHostAddress);
            var robotsFile = "~/robots.txt";
            return File(robotsFile, "text/plain");
        }

        [AllowAnonymous]
        [Route("sitemap.xml")]
        public ActionResult SiteMap()
        {
            logger.Info("SITEMAP: " + Request.UserHostName + Request.UserHostAddress);
            var robotsFile = "~/sitemap.xml";
            return File(robotsFile, "text/xml");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}