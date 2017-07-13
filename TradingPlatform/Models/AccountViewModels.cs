using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Data;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.TradeModel;

namespace TradingPlatform.Models
{
    public class UploadTradePassportModel
    {
        public int TradeNumber { get; set; }
        public HttpPostedFileBase ProductPassport { get; set; }
    }
    public class SendMailModel
    {
        public List<SelectListItem> PreparedEmails { get; set; }
        public string Mails { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
    /// <summary>
    /// Модель для заполнения счетов, договоров и прочей дряни
    /// </summary>
    public class ShowTradeDocsModel
    {
        public ShowTradeDocsModel()
        {
            
        }
        public ShowTradeDocsModel(TradeBill bill)
        {
            TradeNumber = bill.TradeId;
            DocCreaDate = bill.CreateDate;
            Contract = bill.Contractt.ContractNumber;
            AddContract = bill.AddContractName;
            BillNumber = bill.BillName;
            SellerName = bill.Seller.LongName;
            SellerNameEng = bill.Seller.LongNameEng;
            BuyerName = bill.Buyer.LongName;
            BuyerNameEng = bill.Buyer.LongNameEng;
            Id = bill.Id;
        }
        /// <summary>
        /// Айдишник из БД TradeBill, на основании которого происходит генерация
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Номер торгов
        /// </summary>
        [LDisplayNameAttribute("attribute", "tradeNumber", "№ торгів", "№ торгов")]
        public int TradeNumber { get; set; }
        /// <summary>
        /// Когда создан документ
        /// </summary>
        [LDisplayNameAttribute("attribute", "DocCreaDate", "Дата створення", "Дата создания")]
       
        public DateTime DocCreaDate{ get; set; }
        // [LDisplayNameAttribute("attribute", "BillLink", "Рахунок", "Счет")]
        // public string BillLink { get; set; }
        /// <summary>
        /// Название договора
        /// </summary>
        [LDisplayNameAttribute("attribute", "DocLink", "Договір", "Договор")]
        public string Contract { get; set; }
        /// <summary>
        /// Название дополнительного договора поставки
        /// </summary>
        [LDisplayNameAttribute("attribute", "AddDocLink", "Додаткова угода", "Дополнительное соглашение")]
        public string AddContract { get; set; }
        /// <summary>
        /// Номер счета
        /// </summary>
        [LDisplayNameAttribute("attribute", "billNum", "Рахунок", "Счет")]
        public string BillNumber { get; set; }
        /// <summary>
        /// Наименование компании продавца (Longname)
        /// </summary>
        [LDisplayNameAttribute("attribute", "SellerName", "Продавець", "Продавец")]
        public string SellerName { get; set; }
        public string SellerNameEng { get; set; }
        /// <summary>
        /// Наименование компании покупателя (Longname)
        /// </summary>
        [LDisplayNameAttribute("attribute", "BuyerName", "Покупець", "Покупатель")]
        public string BuyerName { get; set; }
        public string BuyerNameEng { get; set; }


    }

    public class BillModel
    {
        public ContragentViewModel Seller { get; set; }
        public ContragentViewModel Buyer { get; set; }
        public TradeViewModel Trade { get; set; }
        /// <summary>
        /// Моделька, в которой инфа о счетах/договорах
        /// </summary>
        public ShowTradeDocsModel ShowModel { get; set; }
        /// <summary>
        /// Список ставок 
        /// </summary>
        public List<BetViewModel> Bets { get; set; } 
        public string HtmlContent { get; set; }
    
    }



    public class EditProfileModel
    {
        [LDisplayNameAttribute("attribute","userName","Ім’я","Имя")]
        public string Name { get; set; }
        [LDisplayNameAttribute("attribute", "userPatr", "По-батькові", "Отчество")]
        public string Patronimyc { get; set; }
        [LDisplayNameAttribute("attribute", "userSurn", "Прізвище", "Фамилия")]
        public string Surname { get; set; }
        [LDisplayNameAttribute("attribute", "userMail", "E-mail", "E-mail")]
        public string Email { get; set; }
        [LDisplayNameAttribute("attribute", "userAddMail", "Додатковий e-mail", "Дополнительный e-mail")]
        public string AdditionalEmail { get; set; }
        [LDisplayNameAttribute("attribute", "userPhone", "Телефон", "Телефон")]
        public string Phone { get; set; }
        [LDisplayNameAttribute("attribute", "AdditionalPhone", "Додатковий телефон", "Дополнительный телефон")]
        public string AdditionalPhone { get; set; }
        [LDisplayNameAttribute("attribute", "AllowPromoMails", "Отримувати промо-інформацію", "Получать промо-информацию")]
        public bool AllowPromoMails { get; set; }
        [LDisplayNameAttribute("attribute", "AllowTradeMails", "Отримувати інформацію про торги", "Получать информацию о торгах")]
        public bool AllowTradeMails { get; set; }
        [LDisplayNameAttribute("attribute", "AllowImportantMails", "Отримувати важливу інформацію", "Получать важную информацию")]
        public bool AllowImportantMails { get; set; }

    }
    public class CalendarModel
    {
        public string Day { get; set; }
        public bool Active { get; set; }
    }
    public class SignalMessage
    {
        public string Name { get; set; } // Имя пользователя
        public string Message { get; set; } // Сообщение пользователя
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [RequiredLocalized("error","emailrequir","Тут не повинно бути пусто","Тут не должно быть пусто")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool IsAccepted { get; set; }
        public bool ShowOffert { get; set; }

    }

    public class ApproveModel
    {
        public ApproveModel(ContragentViewModel model)
        {
            ContragentId = model.Id;
            ContragentFiles = model.Documents.Select(c => c).ToList();
            IsApproved = model.IsApproved;
            Comment = model.ApprovingComment;
            IsBuyer = model.IsBuyer;
            IsSeller = model.IsSeller;
            HasContractCopy = model.HasContractCopy;
            HasContractOriginal = model.HasContractOriginal;
            ContragentName = model.LongName;
            ContractIsOnSign = model.ContractOnSignin;
        }

        public ApproveModel()
        {

        }
        public string ContragentName { get; set; }
        public int ContragentId { get; set; }
        public List<FileViewModel> ContragentFiles { get; set; }
        public bool IsApproved { get; set; }
        public bool HasContractCopy { get; set; }
        public bool HasContractOriginal { get; set; }
        public string Comment { get; set; }
        public bool IsBuyer { get; set; }
        public bool ContractIsOnSign { get; set; }
        public bool IsSeller { get; set; }
        public bool SendMail { get; set; }

    }

    public class RegisterContragentModel
    {
        public RegisterContragentModel()
        {
            DocumentsList = new List<RequiredDocs>();
        }
        [Required]
        public string Name { get; set; }

        public bool IsTaxPayer { get; set; }

        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }

        [Required]
        public int Code { get; set; } //
        [Required]
        public string TaxCode { get; set; } //

        [Required]
        public string CeoTitle { get; set; }
        [Required]
        public string CeoName { get; set; }

        public bool IsConfidant { get; set; } //
        public string ConfidantName { get; set; }
        public string ConfidantDoc { get; set; }
        
        public bool IsoptionalBankBill { get; set; } //

        public List<RequiredDocs> DocumentsList { get; set; }

        [Required]
        public BankBillViewModel BankBillRequired { get; set; }
        public BankBillViewModel BankBillOptional { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string ContragentEmail { get; set; }

        [DataType(DataType.Url)]
        public string ContragentSite { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContragentPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string ContragentFax { get; set; }

        public AddressViewModel Address { get; set; }
        public AddressViewModel PostAddress { get; set; }

    }

    public class RequiredDocs
    {
        public string DocumentAlias { get; set; }
        //[Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase DocumentFile { get; set; }
    }
    public class ContragentAddress
    {
        public string ZipCode { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string House { get; set; }
        public bool IsEqualsToPostAddress { get; set; }
    }

    public class ContragentPostAddress
    {
        public string AbonentBox { get; set; }
        public string ZipCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
    }

    public class Ft
    {
        public  int Type { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
    }

    
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
      //  [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
      //  [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool Confirm { get; set; }
        [Required]
       // [StringLength(14, ErrorMessage = "The phone number {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Phone { get; set; }

        [Required]
        public string Name { get; set; }

        [RegularExpression(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$",ErrorMessage = "Wrong ip")]
        public string Ip { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Ip", ErrorMessage = "The IP and confirmation of IP do not match.")]
        public string ConfirmIp { get; set; }
        public string DetectedIp { get; set; }

        public string Role { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Token { get; set; }
    }

    public class StuffUserViewModel
    {
        public string Email { get; set; }
        public  List<string> Roles { get; set; }
    }
    //public class ChangePasswordViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}
}

