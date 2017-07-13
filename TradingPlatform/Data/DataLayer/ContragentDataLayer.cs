using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.ProfileModel;

namespace TradingPlatform.Data.DataLayer
{
    public class ContragentDataLayer
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ApplicationDbContext db { get; private set; }

        public ContragentDataLayer(ApplicationDbContext ctx) 
        {
            db = ctx;
        }
        public List<Contragent> GetContragents(bool? IsApproved = null)
        {
            
                var result = db.Contragents.OrderByDescending(c => c.UpdateDate).AsQueryable();

                if (IsApproved.HasValue)
                    result = result.Where(x => x.IsApproved == IsApproved.Value);

                return result.ToList();
            
        }
   
        public ContragentViewModel GetContragent(int id)
        {
            return FillContragent(db.Contragents.SingleOrDefault(c => c.Id == id));
        }

        public List<FileViewModel> GetDocuments(int contragentId)
        {
            return db.Files.Where(d => d.ContragentId == contragentId).ToList().Select(FillDocuments).ToList();
        }

        public List<Bet> GetCurrentUserBets(Trade trade, int contragentid)
        {
            return trade.Bets.Where(c => c.BuyerId == contragentid).ToList();
        }

        public ContragentViewModel FillContragent(Contragent contragent)
        {
            if (contragent == null)
                return new ContragentViewModel();
            else
                return new ContragentViewModel()
                {
                    Id = contragent.Id,
                    LongName = contragent.LongName,
                    GroupName = contragent.Group != null ? contragent.Group.GroupName : "",
                    Code = contragent.Code.ToString(),
                    TaxCode = contragent.TaxCode,
                    TaxType = contragent.TaxType,
                    ContragentEmail = contragent.ContragentEmail,
                    ContragentSite = contragent.ContragentSite,
                    ContragentPhone = contragent.ContragentPhone,
                    ContragentFax = contragent.ContragentFax,
                    //ContractIsOnSign = contragent.ContractOnSignin,
                    CreateDate = contragent.CreateDate,
                    UpdateDate = contragent.UpdateDate,

                    IsApproved = contragent.IsApproved,
                    IsTaxPayer = contragent.IsTaxPayer,
                    IsSeller = contragent.IsSeller,
                    IsBuyer = contragent.IsBuyer,
                    IsConfidant = contragent.IsConfidant,
                    HasContractCopy = contragent.HasContractCopy,
                    HasContractOriginal = contragent.HasContractOriginal,
                    ContractOnSignin = contragent.ContractOnSignin,
                    ApprovingComment = contragent.ApprovingComment,
                    CeoTitle = contragent.CeoTitle,
                    CeoName = contragent.CeoName,
                    ConfidantName = contragent.ConfidantName,
                    ConfidantDocumentType = contragent.ConfidantDocument,

                    BankBillRequired = contragent.BankBills.Where(b => b.IsPrimary).ToList().Select(c=>new BankBillViewModel(c)).FirstOrDefault() ?? new BankBillViewModel() { ContragentId = contragent.Id },
                    BankBillOptional = contragent.BankBills.Where(b => !b.IsPrimary).ToList().Select(c => new BankBillViewModel(c)).FirstOrDefault() ?? new BankBillViewModel() { ContragentId = contragent.Id },
                    IsOptionalBankBill = contragent.BankBills.Any(b => !b.IsPrimary),

                    Documents = contragent.Documents.Select(d => FillDocuments(d)).ToList(),

                    Address = contragent.Address,//FillAddress(address: contragent.Address),
                    PostAddress = contragent.PostAddress,//FillAddress(postAddress: contragent.PostAddress),

                    ContragentUsers = contragent.ContragentUsers.Select(u => FillUser(u)).ToList(),
                    CreatedByUser = FillUser(contragent.CreatedByUser),
                    ApprovedByUser = FillUser(contragent.ApprovedByUser),
                    IsDutyPayer = contragent.IsDutyPayer,
                    IsResident = contragent.IsResident,
                    ConfidantNameEng = contragent.ConfidantNameEng,
                    AddressEng = contragent.AddressEng,
                    ConfidantDocumentTypeEng = contragent.ConfidantDocumentEng,
                    LongNameEng = contragent.LongNameEng
                    
                };
        }

      
        public UserViewModel FillUser(ApplicationUser user)
        {
            if (user == null)
                return new UserViewModel();
            else
                return new UserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName
                };
        }
              

        public AddressViewModel FillAddress(Address address = null, PostAddress postAddress = null)
        {
            AddressViewModel result = new AddressViewModel();
            if (address != null)
            {
                result.Id = address.Id;
                result.Region = address.Region;
                result.City = address.City;
                result.Street = address.Street;
                result.House = address.House;
                result.ZipCode = address.ZipCode;
            }
            else if (postAddress != null)
            {
                result.Id = postAddress.Id;
                result.Region = postAddress.Region;
                result.City = postAddress.City;
                result.Street = postAddress.Street;
                result.House = postAddress.House;
                result.ZipCode = postAddress.ZipCode;
                result.AbonentBox = postAddress.AbonentBox;
            }
            return result;
        }

        public FileViewModel FillDocuments(File doc)
        {
            if (doc == null)
                return new FileViewModel();
            else
                return new FileViewModel()
                {
                    Id = doc.Id,
                    FileName = doc.FileName,
                    IsApproved = doc.IsApproved,
                    Comment = doc.Comment,
                    FileType = FillFileType(doc.FileType),
                    CreateDate = doc.CreateDate,
                    UpdateDate = doc.UpdateDate,
                    Content = doc.Content
                };
        }

        public FileTypeViewModel FillFileType(FileType type)
        {
            if (type == null)
                return new FileTypeViewModel();
            else
                return new FileTypeViewModel()
                {
                    Id = type.Id,
                    Alias = type.Alias,
                    Name = type.Name,
                    Desc = type.Desc
                };
        }



    }
}