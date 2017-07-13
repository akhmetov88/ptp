using System.ComponentModel.DataAnnotations;
using TradingPlatform.Data;

namespace TradingPlatform.Models.EntityModel
{
    public class BankBillViewModel
    {
        public BankBillViewModel()
        {
            
        }
        public BankBillViewModel(BankBill bank)
        {
            Id = bank.Id;
            Mfo = bank.Mfo;
            BankName = bank.BankName;
            BankNameEng = bank.BankNameEng;
            IsPrimary = bank.IsPrimary;
            BillNumber = bank.BillNumber;
            ContragentId = bank.ContragentId;
            IsResident = bank.IsResident;
            IsCorrespondent = bank.IsCorrespondent;
            Iban = bank.Iban;
            Swift = bank.Swift;
            Address = bank.Address;
            AddressEng = bank.AddressEng;
        }
        public int Id { get; set; }
      ///  [RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string BankName { get; set; }
        ///[RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string BankNameEng { get; set; }

        [RegularExpression("[1-9][0-9]{5}", ErrorMessage = "Невірний формат")]
        public string Mfo { get; set; }
        ///[RequiredLocalizedAttribute("error", "fieldReauired", "Будь ласка, введіть дані", "Пожалуйста, введите данные")]
        public string BillNumber { get; set; }
        public bool IsResident { get; set; }
        
        public bool IsCorrespondent { get; set; }

        public string Iban { get; set; }
        public string Swift { get; set; }

        public string BankCountry { get; set; }
        public string Address { get; set; }
        public string AddressEng { get; set; }

        public bool IsPrimary { get; set; }

        public int ContragentId { get; set; }

#warning В модели банка закомментирован контрагент, добавлен просто айдишник контрагента

        // public ContragentViewModel Contragent { get; set; }
    }
}
