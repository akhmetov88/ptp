using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using TradingPlatform.Controllers;
using TradingPlatform.Data;
using TradingPlatform.Data.DataLayer;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.NotifyModels;
using System.Threading.Tasks;
using TradingPlatform.Messaging;

namespace TradingPlatform.Areas.Lawyer.Controllers
{
    [Authorize(Roles = "lawyer,admin,root")]
    public class ContragentController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ContragentDataLayer _dl;

        private ApplicationDbContext _db;

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


        private ApplicationSignInManager _signInManager;
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

        [InLeftMenu]
        public ActionResult Index()
        {
            return View(dl.GetContragents(false));
        }

        [InLeftMenu]
        public ActionResult History()
        {

            var model = dl.GetContragents(true).ToList();
            return View(model);
        }

        [InLeftMenu]
        public ActionResult Details(int id)
        {
            var model = dl.GetContragent(id);

            var docsTypes = db.FileTypes.Where(c => c.Alias != "QualityPassport").ToList();

            foreach (var item in docsTypes)
            {
                if (!model.Documents.Any(x => x.FileType.Id == item.Id))
                {
                    model.Documents.Add(new FileViewModel()
                    {
                        Id = 0,
                        IsApproved = true,
                        FileName = string.Empty,
                        FileType = new FileTypeViewModel()
                        {
                            Id = item.Id,
                            Alias = item.Alias,
                            Name = item.Name,
                            Desc = item.Desc,
                        },

                    });
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(ApproveModel model)
        {

            try
            {
                var currentuser = UserManager.FindById(User.Identity.GetUserId());
                var cont = db.Contragents.Single(x => x.Id == model.ContragentId);
                cont.ApprovingComment = model.Comment;
                cont.HasContractCopy = model.HasContractCopy;
                cont.HasContractOriginal = model.HasContractOriginal;
                cont.IsApproved = model.IsApproved;
                cont.IsBuyer = model.IsBuyer;
                cont.IsSeller = model.IsSeller;
                cont.ApprovedByUserId = currentuser.Id;
                cont.ApprovedByUser = currentuser;
                cont.ContractOnSignin = model.ContractIsOnSign;
                db.UpdateEntity(cont, currentuser.Id);
       
                if (cont.IsApproved)
                {
                    CheckRoles(cont);

                     EmailFactory.SendEmailAsync(new LegalActive(cont.CreatedByUser, cont));
          
                }
                else
                {
                    CheckRoles(cont);
                    if (model.SendMail)
                    {
                        foreach (var user in cont.ContragentUsers.ToList())
                        {
                            logger.Info($"contragent {cont.LongName} deactivated, sending mail");

                             EmailFactory.SendEmailAsync(new LegalNotActive(user, cont));
                     
                        }
                    }

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Details", new { @id = model.ContragentId });
            }

        }

        private void CheckRoles(Contragent cont)
        {
           
            foreach (var user in cont.ContragentUsers.ToList())
            {
                if (cont.IsBuyer)
                {
                    var t = UserManager.AddToRole(user.Id, "byer");
                    logger.Info($"{user.Email} from contragent {cont.LongName} adds to byer");
                    if (!t.Succeeded)
                    {
                        foreach (var err in t.Errors)
                        {
                            logger.Error(err);
                        }
                    }
                }
                else
                {
                    if (user.UserContragents.Count(c => c.IsBuyer) < 1)
                    {
                        var t = UserManager.RemoveFromRole(user.Id, "byer");
                        logger.Info($"{user.Email} from contragent {cont.LongName} remove from  byers");
                        if (!t.Succeeded)
                        {
                            foreach (var err in t.Errors)
                            {
                                logger.Error(err);
                            }
                        }
                    }

                }
                if (cont.IsSeller)
                {
                    var t = UserManager.AddToRole(user.Id, "trader");
                    logger.Info($"{user.Email} from contragent {cont.LongName} adds to trader");
                    if (!t.Succeeded)
                    {
                        foreach (var err in t.Errors)
                        {
                            logger.Error(err);
                        }
                    }
                }
                else
                {
                    if (user.UserContragents.Count(c => c.IsSeller) < 1)
                    {
                        var t = UserManager.RemoveFromRole(user.Id, "trader");
                        logger.Info($"{user.Email} from contragent {cont.LongName} remove from  sellers");
                        if (!t.Succeeded)
                        {
                            foreach (var err in t.Errors)
                            {
                                logger.Error(err);
                            }
                        }
                    }
                }
                //  SignInManager.SignIn(user, false, false);

            }
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [InLeftMenu]
        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult ShowFile(int file)
        {
            try
            {
                var fileToRetrieve = db.Files.Find(file);
                return File(fileToRetrieve.Content, fileToRetrieve.ContentType, fileToRetrieve.FileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }

        }
    }
}
