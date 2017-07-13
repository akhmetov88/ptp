using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TradingPlatform.Data;
using TradingPlatform.Helpers;
using TradingPlatform.Models.ProfileModel;

namespace TradingPlatform.Models.EntityModel
{
    public class ContragentViewModel
    {
        public ContragentViewModel()
        {
            BankBillRequired = new BankBillViewModel();
            BankBillOptional = new BankBillViewModel();
            IsTaxPayer = true;
        }

        public int Id { get; set; }

        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string LongName { get; set; }
        public string LongNameEng { get; set; }

        public string GroupName { get; set; }

        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        [RegularExpression("[1-9][0-9]{7,9}", ErrorMessage = "Невірний формат")]
        public string Code { get; set; }

        public string ApprovingComment { get; set; }

        public bool IsEqualsToPostAddress { get; set; }

        //[RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        [RegularExpression("[1-9][0-9]{9,11}", ErrorMessage = "Невірний формат")]
        public string TaxCode { get; set; }

        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string ContragentEmail { get; set; }
        public string ContragentSite { get; set; }
        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string ContragentPhone { get; set; }
        public string ContragentFax { get; set; }

        public bool IsApproved { get; set; }
        public bool IsTaxPayer { get; set; }
        public bool IsDutyPayer { get; set; }

        public bool HasContractCopy { get; set; }
        public bool HasContractOriginal { get; set; }
        public bool ContractOnSignin { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string TaxType { get; set; }
        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        //   [RegularExpression(@".{10,}", ErrorMessage = "Введіть, як мінімум, ім’я та по-батькові")]
        public string CeoTitle { get; set; }

        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        [RegularExpression(@".{10,}", ErrorMessage = "Введіть, як мінімум, ім’я та по-батькові")]
        public string CeoName { get; set; }

        public bool IsConfidant { get; set; }

        public string ConfidantName { get; set; }
        public string ConfidantNameEng { get; set; }

        public string ConfidantDocumentType { get; set; }
        public string ConfidantDocumentTypeEng { get; set; }

        public List<FileViewModel> Documents { get; set; }

        public BankBillViewModel BankBillRequired { get; set; }
        public bool IsOptionalBankBill { get; set; }
        public bool IsOptionalBankBillEng { get; set; }

        public BankBillViewModel BankBillOptional { get; set; }

        public List<UserViewModel> ContragentUsers { get; set; }

        public UserViewModel CreatedByUser { get; set; }
        public UserViewModel ApprovedByUser { get; set; }
        [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string Address { get; set; }
        public string AddressEng { get; set; }

        public string PostAddress { get; set; }
        public string PostAddressEng { get; set; }


        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase edrpou { get; set; }
        [MaximumFileSizeValidator(45)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase statute { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase singletaxcertificate { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase taxcodecertificate { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase bankbillcert { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase secondbankbillcert { get; set; }

        //[RequiredLocalizedAttribute("error", "fileRequired", "Будь ласка, завантажте файл", "Пожалуйста, загрузите файл")]
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase ceoDocument { get; set; }

        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase confidantDocument { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase taxForm { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase singletaxcodecertificate { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase docnotpay { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase contragentdutypayerdoc { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase protocoldocument { get; set; }
        [MaximumFileSizeValidator(15)]
        [ValidFileTypeValidator("png", "jpg", "jpeg", "pdf", "tiff", "rtf", "doc", "docx")]
        public HttpPostedFileBase QualityPassport { get; set; }

        public ICollection<HttpPostedFileBase> Files { get; set; }
        public bool IsResident { get; set; }
        public string ResidenseCountry { get; set; }
        public string ResidenseCountryEng { get; set; }
    }

    public class ContragentTradeViewModel
    {
        public ContragentTradeViewModel()
        {
            
        }
        public ContragentTradeViewModel(Contragent contragent)
        {
            Id = contragent.Id;
            LongName = contragent.LongName;
            BankBills = contragent.BankBills.Select(c => new BankBillViewModel(c)).ToList();
        }
        public int Id { get; set; }
        public string LongName { get; set; }

        public List<BankBillViewModel> BankBills { get; set; }

    }
}
