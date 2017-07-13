using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TradingPlatform.Data;
using TradingPlatform.Models;
using TradingPlatform.Data.DataLayer;
using System.IO;
using System.Web.Security;
using NLog;
using reCAPTCHA.MVC;
using TradingPlatform.Enums;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Messaging;

namespace TradingPlatform.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AccountController : BaseController
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
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, PtpRolesManager rolesManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RolesManager = rolesManager;
        }
        #endregion
        [HttpGet]
        [System.Web.Mvc.Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(RegisterViewModel model)
        {
            return View();
        }
       

        #region заявки на аккредитацию
        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin"), InLeftMenu]
        public ActionResult Index()
        {

            try
            {

                var user = UserManager.FindById(User.Identity.GetUserId());
                var contragents = db.Contragents.Where(c => c.GroupId == user.GroupId).ToList();
                var approvingmodel = contragents.Select(contr => new ApproveModel()
                {
                    ContragentId = contr.Id,
                    ContragentName = contr.LongName,
                    IsApproved = contr.IsApproved,
                    IsBuyer = contr.IsBuyer,
                    IsSeller = contr.IsSeller,
                    Comment = contr.ApprovingComment,
                }).ToList();
                return View(approvingmodel);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View();
            }


        }
        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin"), InLeftMenu]
        public ActionResult EditProfile()
        {
            try
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                if (user == null)
                {
                    return HttpNotFound();
                }
                var model = new EditProfileModel()
                {
                    Patronimyc = user.UserInfo?.Patronymyc,
                    AdditionalPhone = user.UserInfo?.AdditionalPhone,
                    Phone = user.PhoneNumber,
                    Surname = user.UserInfo?.Surname,
                    Name = user.UserInfo?.Name ?? user.RegisterName ?? user.UserName,
                    AdditionalEmail = user.UserInfo?.AdditionalMail,
                    Email = user.Email,
                    AllowImportantMails = true,
                    AllowPromoMails = user.AllowPromoEmails,
                    AllowTradeMails = user.AllowTradeEmails                
                };
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = UserManager.FindByEmail(model.Email);
                    if (user.UserInfo == null)
                    {
                        var userInfo = new UserInfo()
                        {
                            User = user,
                            AdditionalMail = model.AdditionalEmail,
                            AdditionalPhone = model.AdditionalPhone,
                            Name = model.Name,
                            Surname = model.Surname,
                            Patronymyc = model.Patronimyc,

                        };
                        db.Insert(userInfo, User.Identity.GetUserId());

                        user.PhoneNumber = model.Phone;
                       // user.RegisterName = $"{model.Name} {model.Surname}";
                        UserManager.Update(user);
                    }
                    else
                    {
                        user.UserInfo.AdditionalMail = model.AdditionalEmail;
                        user.UserInfo.AdditionalPhone = model.AdditionalPhone;
                        user.UserInfo.Name = model.Name;
                        user.UserInfo.Surname = model.Surname;
                        user.UserInfo.Patronymyc = model.Patronimyc;
                        user.PhoneNumber = model.Phone;
                        user.AllowTradeEmails = model.AllowTradeMails;
                        user.AllowPromoEmails = model.AllowPromoMails;
                    }
                    user.PhoneNumber = model.Phone;
                    user.UserName = $"{model.Name} {model.Surname}";
                    UserManager.Update(user);
                    return RedirectToAction("EditProfile");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            return View(model);

        }
        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin")]
        [HttpGet]
        public ActionResult ShowFile(int file, ApplicationDbContext db)
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

        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin")]
        [HttpPost]
        public ActionResult ChangeFile(HttpPostedFileBase doc, int id, int filetypeid, ApplicationDbContext db)
        {
            try
            {
                var existedfile = db.Files.SingleOrDefault(c => c.Id == id && c.FileTypeId == filetypeid);
                if (existedfile != null)
                {
                    using (var reader = new System.IO.BinaryReader(doc.InputStream))
                    {
                        existedfile.Content = reader.ReadBytes(doc.ContentLength);
                    }
                    existedfile.FileName = doc.FileName;
                    existedfile.CreateDate = existedfile.UpdateDate;
                    existedfile.UpdateDate = DateTime.UtcNow;
                    //предлагаю писать варнинги вместо ТУДУ
                    db.UpdateEntity(existedfile, User.Identity.GetUserId());
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Не удалось обновить файл");
                return View("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("", "Не удалось обновить файл");
                return View("Index");
            }

        }

        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin")]
        public ActionResult Create(bool isResident = true)
        {
            ViewBag.TaxTypes =
                   db.Catalogs.SingleOrDefault(c => c.ParentId == null && c.Code == "taxTypes")?.Dependencies.Select(c => new SelectListItem()
                   {
                       Value = c.Code,
                       Text = StaticData.Inst.GetCatalogValue(c.Code)
                   }).ToList();
            var model = new ContragentViewModel() { IsResident = isResident };
            return isResident ? View(model) : View("CreateNonResident",model);
        }

        //   [Authorize(Roles = "trader,user,contragent,byer,root,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContragentViewModel contragentModel)
        {
            try
            {
                db.Database.CommandTimeout = 0;
                var user = db.Users.Find(User.Identity.GetUserId());
                if (db.Contragents.Any(c => c.LongName == contragentModel.LongName || c.Code == contragentModel.Code))
                {
                    logger.Error($"Same contragent with name{contragentModel.LongName} and code {contragentModel.Code}");
                    ModelState.AddModelError("LongName",
                        LocalText.Inst.Get("error", "contragentalreadyInserted",
                            "Компанію з такою назвою чи ідентифікаційним кодом вже зареєстровано",
                            "Компания с таким названием или идентификационнім кодом уже зарегистрирована"));
                    ModelState.AddModelError("Code",
                        LocalText.Inst.Get("error", "contragentalreadyInserted",
                            "Компанію з такою назвою чи ідентифікаційним кодом вже зареєстровано",
                            "Компания с таким названием или идентификационнім кодом уже зарегистрирована"));
                }
                if (!contragentModel.IsResident)
                    ModelState.Clear();



                if (ModelState.IsValid && user != null)
                {
                    logger.Info($"Createing contragent {contragentModel.LongName}");
                    Group group;
                    if (String.IsNullOrEmpty(User.Identity.GetUserGroup()) || !user.GroupId.HasValue)
                    {
                        group = new Group()
                        {
                            GroupName = contragentModel.LongName
                        };
                        db.Insert(group, User.Identity.GetUserId());
                        user.Group = group;
#warning look at update user
                        db.SaveChanges();

                        //db.UpdateEntity(user, User.Identity.GetUserId());
                    }
                    else
                    {
                        group = UserManager.GetUserGroup(user);
                    }


                    #region banks

                    List<BankBill> banks = new List<BankBill>();
                    banks.Add(new BankBill()
                    {
                        BankName = contragentModel.BankBillRequired.BankName,
                        BillNumber = contragentModel.BankBillRequired.BillNumber,
                        IsPrimary = true,
                        Mfo = contragentModel.BankBillRequired.Mfo
                    });
                    if (contragentModel.IsOptionalBankBill)
                        banks.Add(new BankBill()
                        {
                            BankName = contragentModel.BankBillOptional.BankName,
                            BillNumber = contragentModel.BankBillOptional.BillNumber,
                            IsPrimary = false,
                            Mfo = contragentModel.BankBillOptional.Mfo
                        });

                    #endregion

                    #region contragent

                    var contragent = new Contragent()
                    {
                        LongName = contragentModel.LongName,
                        TaxCode = contragentModel.TaxCode,
                        Code = contragentModel.Code,
                        ContragentEmail = contragentModel.ContragentEmail,
                        ContragentSite = contragentModel.ContragentSite,
                        ContragentPhone = contragentModel.ContragentPhone,
                        ContragentFax = contragentModel.ContragentFax,

                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,

                        IsApproved = false,
                        IsTaxPayer = contragentModel.IsTaxPayer,
                        IsSeller = contragentModel.IsSeller,
                        IsBuyer = contragentModel.IsBuyer,
                        IsConfidant = contragentModel.IsConfidant,

                        CeoTitle = contragentModel.CeoTitle,
                        CeoName = contragentModel.CeoName,
                        ConfidantName = contragentModel.ConfidantName,
                        ConfidantDocument = contragentModel.ConfidantDocumentType,
                        BankBills = banks,
                        Address = contragentModel.Address,
                        PostAddress =
                            contragentModel.IsEqualsToPostAddress
                                ? contragentModel.Address
                                : contragentModel.PostAddress,

                        ContragentUsers = UserManager.Users.Where(c => c.Id == user.Id).ToList(),
                        CreatedByUser = user,
                        CreatedByUserId = user.Id,
                        Group = group,
                        GroupId = group.Id,
                        IsDutyPayer = contragentModel.IsDutyPayer,
                        TaxType = contragentModel.TaxType,
                        HasContractOriginal = contragentModel.HasContractOriginal,
                        HasContractCopy = contragentModel.HasContractCopy,
                        ContractOnSignin = contragentModel.ContractOnSignin,
                        IsResident = contragentModel.IsResident

                    };
                    #endregion
                    db.Insert(contragent, User.Identity.GetUserId());
                    #region docs

                    foreach (string file in Request.Files)
                    {
                        var fileTypeId =
                            db.FileTypes.SingleOrDefault(c => c.Alias.ToLower() == file.ToLower())?.Id; //??
                                                                                                        //CreateFileType(file.ToLower(), db).Id;
                        if (!String.IsNullOrEmpty(file) && Request.Files[file].ContentLength > 0)
                        {
                            var uplFile = new Data.File
                            {
                                FileName = Path.GetFileName(Request.Files[file].FileName),
                                ContentType = Request.Files[file].ContentType,
                                UserId = user.Id,
                                User = user,
                                IsApproved = false,
                                Comment = "",
                                FileTypeId = fileTypeId,
                                CreateDate = DateTime.UtcNow,
                                UpdateDate = DateTime.UtcNow,
                                ContragentId = contragent.Id,
                                Content =
                                    Request.Files[file].ContentLength > 0
                                        ? GetContentFromPostedFile(Request.Files[file].InputStream)
                                        : null
                            };
                            if (uplFile.Content != null)
                            {
                                logger.Info($"Try insert posted file {file} ({uplFile.FileName})");
                                db.Insert(uplFile, User.Identity.GetUserId());
                            }
                        }
                    }

                    #endregion

                    user.UserContragents.Add(contragent);
                    user.GroupId = group.Id;
                    UserManager.Update(user);

                    db.SaveChanges();
                 
                    string link = Url.Action("Details", "Contragent", new { id = contragent.Id, area = "Lawyer" }, protocol: Request.Url.Scheme);
                    string link1 = Url.Action("Index", "Account");

                    EmailFactory.SendEmailAsync(new ToLawyerAboutNewCompany(user, contragent.LongName, link));
                    EmailFactory.SendEmailAsync(new ToUserAboutNewCompany(user, contragent.LongName, link1));             

                    return RedirectToAction("RegisterThanksJuridical", "Home");
                }
                ViewBag.TaxTypes =
                   StaticData.Inst.GetAllCatalogsList().SingleOrDefault(c => c.ParentId == null && c.Code == "taxTypes")?
                        .Dependencies.Select(c => new SelectListItem()
                        {
                            Value = c.Code,
                            Text = StaticData.Inst.GetCatalogValue(c.Code)
                        }).ToList();
                if (contragentModel.IsResident)
                    return View(contragentModel);
                else
                    return View("CreateNonResident", contragentModel);

            }

            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", ex.Message);
                return View("Error", ModelState);
            }
        }





        [System.Web.Mvc.Authorize()]
        public ActionResult Upl()
        {
            var tradedb = db.Trades.Find(32);
            if (tradedb != null)
            {
                var model = new UploadTradePassportModel()
                {
                    TradeNumber = tradedb.Id,
                };
                return View(model);

            }
            return View("Error");
        }
        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult Upl(UploadTradePassportModel model)
        {
            foreach (string file in Request.Files)
            {
                try
                {
                    var trade = db.Trades.Find(model.TradeNumber);
                    if (trade != null)
                    {
                        trade.ProductPassport.Content = GetContentFromPostedFile(Request.Files[file].InputStream);
                        db.SaveChanges();
                        //db.UpdateEntity(trade.ProductPassport);
                    }
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View("Error", ModelState);

                }

            }
            return View();
            // var user = db.GetCurrentUser(User.Identity.GetUserId());

        }

        private byte[] GetContentFromPostedFile(Stream content)
        {
            try
            {
                using (var reader = new System.IO.BinaryReader(content))
                {
                    logger.Info($"GetContent from posted file. Content length = {content.Length} and as integer ={(int)content.Length}");
                    return reader.ReadBytes((int)content.Length);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }


        private FileType CreateFileType(string alias, ApplicationDbContext db)
        {
            try
            {
                var filetype = new FileType() { Alias = alias.ToLower(), Name = alias, Desc = "File type generated automatically: " + alias };
                db.Insert(filetype, User.Identity.GetUserId());
                return filetype;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
        }


        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin"), InLeftMenu]
        public ActionResult Edit(int id)
        {
            ViewBag.TaxTypes = StaticData.Inst.GetAllCatalogsList().SingleOrDefault(c => c.ParentId == null && c.Code == "taxTypes").Dependencies.Select(c => new SelectListItem()
            {
                Value = c.Code,
                Text = StaticData.Inst.GetCatalogValue(c.Code)
            }).ToList();
            return View(Dl.GetContragent(id));
        }


        [System.Web.Mvc.Authorize(Roles = "trader,user,contragent,byer,root,admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(ContragentViewModel contragentModel)
        {
            try
            {
                logger.Info($"Editing contragent {contragentModel.LongName}");
                db.Database.CommandTimeout = 0;
                var user = db.Users.Find(User.Identity.GetUserId());
                if (ModelState.IsValid && user != null)
                {
                    var contr = db.Contragents.Single(x => x.Id == contragentModel.Id);
                    UpdateBankBill(contragentModel.BankBillRequired, true);
                    if (contragentModel.IsOptionalBankBill)
                    {
                        UpdateBankBill(contragentModel.BankBillOptional, false);
                    }

                    #region contragent

                    contr.LongName = contragentModel.LongName;
                    contr.TaxCode = contragentModel.TaxCode;
                    contr.Code = contragentModel.Code;
                    contr.ContragentEmail = contragentModel.ContragentEmail;
                    contr.ContragentSite = contragentModel.ContragentSite;
                    contr.ContragentPhone = contragentModel.ContragentPhone;
                    contr.ContragentFax = contragentModel.ContragentFax;
                    contr.UpdateDate = DateTime.UtcNow;

                    contr.IsApproved = false;
                    contr.IsTaxPayer = contragentModel.IsTaxPayer;
                    contr.IsSeller = contragentModel.IsSeller;
                    contr.IsBuyer = contragentModel.IsBuyer;
                    contr.IsConfidant = contragentModel.IsConfidant;

                    contr.CeoTitle = contragentModel.CeoTitle;
                    contr.CeoName = contragentModel.CeoName;
                    contr.ConfidantName = contragentModel.ConfidantName;
                    contr.ConfidantDocument = contragentModel.ConfidantDocumentType;
                    contr.Address = contragentModel.Address;
                    contr.PostAddress = contragentModel.IsEqualsToPostAddress
                        ? contragentModel.Address
                        : contragentModel.PostAddress;

                    contr.CreatedByUser = user;
                    contr.CreatedByUserId = user.Id;

                    contr.IsDutyPayer = contragentModel.IsDutyPayer;
                    contr.TaxType = contragentModel.TaxType;

                    #endregion

                    db.UpdateEntity(contr, user.Id);

                    #region docs

                    //     List<Data.File> Docs = new List<Data.File>();
                    foreach (string file in Request.Files)
                    {
                        if (Request.Files[file].InputStream.Length > 0)
                        {
                            var fileType =
                                db.Files.FirstOrDefault(
                                    c => c.FileType.Alias.ToLower() == file.ToLower() && c.ContragentId == contr.Id);
                            if (fileType == null)
                            {
                                var fileTypeId =
                                    db.FileTypes.SingleOrDefault(c => c.Alias.ToLower() == file.ToLower())?.Id ??
                                    CreateFileType(file.ToLower(), db).Id;
                                var uplFile = new Data.File
                                {
                                    FileName = Path.GetFileName(Request.Files[file].FileName),
                                    ContentType = Request.Files[file].ContentType,
                                    UserId = user.Id,
                                    User = user,
                                    IsApproved = false,
                                    Comment = "",
                                    FileTypeId = fileTypeId,
                                    CreateDate = DateTime.UtcNow,
                                    UpdateDate = DateTime.UtcNow,
                                    ContragentId = contr.Id,
                                    Content =
                                        Request.Files[file].ContentLength > 0
                                            ? GetContentFromPostedFile(Request.Files[file].InputStream)
                                            : null

                                };
                                if (uplFile.Content != null)
                                {
                                    logger.Info($"Try insert posted file {file} ({uplFile.FileName})");
                                    db.Insert(uplFile, User.Identity.GetUserId());
                                }
                            }
                            else
                            {
                                fileType.Content = GetContentFromPostedFile(Request.Files[file].InputStream);
                                if (fileType.Content != null)
                                {
                                    fileType.User = user;
                                    fileType.FileName = Request.Files[file].FileName;
                                    fileType.UpdateDate = DateTime.UtcNow;
                                    db.SaveChanges();
                                }
                            }
                        }

                    }

                    #endregion

                    if (contragentModel.IsApproved)
                    {
                        foreach (var users in contr.ContragentUsers.ToList())
                        {
                             EmailFactory.SendEmailAsync(new DeactiveMail(users, contr.LongName));
                        }
                    }

                    string link = Url.Action("Details", "Contragent", new { id = contr.Id, area = "Lawyer" },
                        protocol: Request.Url.Scheme);

                    //Mailer.SendUpdateInfoMail("працівник юридичного відділу", contr.LongName, "info@ptp.ua", link);

                     EmailFactory.SendEmailAsync(new UpdateInfoMail(user,contr.LongName,link));

                    return RedirectToAction("EditThanksJuridical", "Home");

                }
                else
                {
                    ViewBag.TaxTypes =
                       StaticData.Inst.GetAllCatalogsList().SingleOrDefault(c => c.ParentId == null && c.Code == "taxTypes")
                            .Dependencies.Select(c => new SelectListItem()
                            {
                                Value = c.Code,
                                Text = StaticData.Inst.GetCatalogValue(c.Code)
                            }).ToList();

                    return View(contragentModel);
                }
            }


            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
                //  ModelState.AddModelError("other", ex);
                //return View("Error", ModelState);
            }
        }

        private void UpdateBankBill(BankBillViewModel bill, bool isPrimary)
        {
            var bank = db.BankBills.SingleOrDefault(x => x.Id == bill.Id);
            if (bank != null)
            {
                bank.BankName = bill.BankName;
                bank.BillNumber = bill.BillNumber;
                bank.IsPrimary = isPrimary;
                bank.Mfo = bill.Mfo;
                db.UpdateEntity(bank, User.Identity.GetUserId());
            }
            else
            {
                var newbank = new BankBill
                {
                    BankName = bill.BankName,
                    BillNumber = bill.BillNumber,
                    IsPrimary = isPrimary,
                    Mfo = bill.Mfo,
                    ContragentId = bill.ContragentId
                };
                db.Insert(newbank, User.Identity.GetUserId());
            }

        }



        #endregion

        [AllowAnonymous]
        public ActionResult Calendar()
        {
            var serverDate = DateTime.UtcNow;

            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            _culture = CultureInfo.GetCultureInfo("ru-RU");
            //_culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;

            var activeday = DateTime.Today.DayOfWeek.ToString();
            var t = _culture.DateTimeFormat;

            var modelList = _culture.DateTimeFormat.ShortestDayNames.
                Select(c => new CalendarModel() { Day = c, Active = (int)serverDate.DayOfWeek == _culture.DateTimeFormat.ShortestDayNames.ToList().IndexOf(c) }).ToList();

            return PartialView(modelList);
        }

        #region авторизация
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.ReturnUrl = returnUrl;
                var model = new LoginViewModel()
                {
                    Email = "",
                    Password = "",
                    RememberMe = false,
                    ShowOffert = false,
                    IsAccepted = false
                };
                return View(model);
            }
            return RedirectToAction("Index");

        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (Request.Form["RememberMe"] != null && Request.Form["RememberMe"] == "on")
                model.RememberMe = true;
            ModelState.Remove("RememberMe");

            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            try
            {
                var user = UserManager.FindByEmail(model.Email);

                if (user == null)
                {
                    throw new ArgumentException(LocalText.Inst.Get("error", "loginErr", "Невірний логін або пароль",
                        "Неверный логин или пароль"));
                }
                if (!user.EmailConfirmed)
                {
                    throw new Exception(LocalText.Inst.Get("error", ",ailNotActive",
                        "Ваш аккаунт не активовано, перевірте свою поштову скриньку та слідуйте інструкціям",
                        "Ваш аккаунт не активирован, проверьте свой почтовый ящик и следуйте инструкциям"));
                }
                if (user.IpRestricted&&user.AllowedIp!=Request.UserHostAddress)
                {
                    throw new Exception(LocalText.Inst.Get("error", "Account.Login.IpResctricted",
                        "Заходити з цієї адреси не дозволено",
                        "Заходить с этого адреса не разрешено"));
                }
                if (!user.IsAcceptedOffert && !model.IsAccepted)
                {
                    model.IsAccepted = false;
                    model.ShowOffert = true;
                    return View(model);
                }
                if (user.IsAcceptedOffert)
                {
                    model.IsAccepted = true;
                }
                if (UserManager.CheckPassword(user, model.Password))
                {
                    var result =
                        await
                            SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe,
                                shouldLockout: true);
                    user.IsAcceptedOffert = model.IsAccepted;
                    UserManager.Update(user);
                    //FormsAuthentication.SetAuthCookie(user.Email,model.RememberMe);
                    //return RedirectToLocal(returnUrl);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            user.LastLoggedIn = DateTime.UtcNow;
                            await UserManager.UpdateAsync(user);
                            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            //FormsAuthentication.SetAuthCookie(user.Email, model.RememberMe);
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode",
                                new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("",
                                LocalText.Inst.Get("error", "loginErr", "Невірний логін або пароль",
                                    "Неверный логин или пароль"));
                            return View(model);
                    }
                }
                else
                {
                    throw new ArgumentException($"wrong password at {model.Email}");
                }
            }


            //  var result = await SignInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe,shouldLockout: false);


            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[CaptchaValidator]
        //Убрал валидацию капчи, захардкодил, что она всегда true
        public async Task<ActionResult> Register(RegisterViewModel model, FormCollection collection, bool captchaValid=true, string returnUrl = null)
        {

            if (/*!model.Confirm||*/!captchaValid)
            {
                ModelState.AddModelError("Captcha", LocalText.Inst.Get("error", "mustAcceptOffert", "Ми не можемо обробити заявку на реєстрацію без акцепту оферти", "Мы не можем обработать заявку на регистрацию без акцепта оферты"));
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
                        await UserManager.AddToRoleAsync(user.Id, "contragent");
                        user.Registered = DateTime.UtcNow;
                        user.AllowPromoEmails = true;
                        user.AllowTradeEmails = true;
                        await UserManager.UpdateAsync(user);
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                            protocol: Request.Url.Scheme);
                        user.IsAcceptedOffert = true;
                        UserManager.Update(user);
                        db.SaveChanges();
                        //Mailer.SendMail(user.Email, "Для подтверждения е-мэйла перейдите по <a href=\"" + callbackUrl + "\">ccылке</a>");
                        //  Mailer.SendRegistrationMail(user.RegisterName ?? model.Name, user.Email, callbackUrl);
                         EmailFactory.SendEmailAsync(new SuccessRegister(user, callbackUrl));// {Link = callbackUrl};
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

                    return RedirectToAction("ConfirmEmail", "Home", new { name = model.Name });
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

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                ModelState.AddModelError("", "Incorrect token");
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(userId);
                SignInManager.SignIn(user, true, true);
                user.LastLoggedIn = DateTime.UtcNow;
                await UserManager.UpdateAsync(user);
            }
            return result.Succeeded ? RedirectToAction("RegisterThanks", "Home") : RedirectToAction("General", "Error");
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