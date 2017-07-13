using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.OfferModels;

namespace TradingPlatform.Models.TradeModel
{

    public class File
    {
        public string name { get; set; }
        public long lastModified { get; set; }
        public string lastModifiedDate { get; set; }
        public string webkitRelativePath { get; set; }
        public int size { get; set; }
        public string type { get; set; }
    }

    public class FileArray
    {
        public string name { get; set; }
        public long lastModified { get; set; }
        public string lastModifiedDate { get; set; }
        public string webkitRelativePath { get; set; }
        public int size { get; set; }
        public string type { get; set; }
    }

    public class fileData
    {
        public string dataURL { get; set; }
        public string base64String { get; set; }
        public File file { get; set; }
        public List<FileArray> fileArray { get; set; }
    }

    public class NewAuctionViewModel
    {
        public NewAuctionViewModel()
        {

        }
        /// <summary>
        /// Конструктор модельки для создания торгов
        /// </summary>
        /// <param name="user">Юзер, у которого будут забираться юрлица</param>
        /// <param name="catalogs">Справочники</param>
        public NewAuctionViewModel(ApplicationUser user, IEnumerable<CatalogModel> catalogs)
        {
            ContragentList = user.UserContragents.Where(c=>c.IsApproved&&c.IsSeller).Select(f=>new ContragentTradeViewModel(f)).ToList();
            TradeTypes = catalogs.Where(c => c.Code.ToLower() == "типы торгов"&&c.IsUsable).ToList();
            CountryList = catalogs.Where(c => c.Code.ToLower() == "страны").ToList();
            ProductsList = catalogs.Where(c => c.Code.ToLower() == "справочник товаров").ToList();
            TransportTypes = catalogs.Where(c => c.Code.ToLower() == "виды транспорта").ToList();
            RailwayTransportTherms = catalogs.Where(c => c.Code.ToLower() == "варианты оборота цистерн").ToList();
            DifferencialPriceDateType = catalogs.SingleOrDefault(c => c.Code == "DifferencialPriceDateType")?.Dependencies.ToList();
            DifferencialPriceText = catalogs.SingleOrDefault(c => c.Code == "DifferencialPriceText")?.Dependencies.ToList();
            DifferencialPriceValueType = catalogs.SingleOrDefault(c => c.Code == "DifferencialPriceValueType")?.Dependencies.ToList();
            DifferencialPriceDateTypeDesc = catalogs.SingleOrDefault(c => c.Code == "DifferencialPriceDateTypeDesc")?.Dependencies.ToList();
            Units = catalogs.SingleOrDefault(c => c.Code.ToLower() == "units")?.Dependencies.ToList();
            Currencies = catalogs.SingleOrDefault(c => c.Code.ToLower() == "валюты")?.Dependencies.ToList();
            TradeBegin = DateTime.Now;
            TradeEnd = DateTime.Now;
            TotalVolume = 0m;
            fileData = new fileData();
        }
        public int? ProductPassportId { get; set; }
        public int? TradeId { get; set; }
        public fileData fileData { get; set; }


        public List<ContragentTradeViewModel> ContragentList { get; set; }
        public ContragentTradeViewModel SelectedSeller { get; set; }
        public BankBillViewModel SelectedBank { get; set; }
        public DateTime TradeBegin { get; set; }
        public DateTime TradeEnd { get; set; }
        public DateTime DeliveryDateFrom { get; set; }
        public DateTime DeliveryDateTo { get; set; }
        /// <summary>
        /// Справочник стран
        /// </summary>
        public List<CatalogModel> CountryList { get; set; }
        /// <summary>
        /// Справочник типов торгов
        /// </summary>
        public List<CatalogModel> TradeTypes { get; set; }
        /// <summary>
        /// Справочник товаров
        /// </summary>
        public List<CatalogModel> ProductsList { get; set; }
        /// <summary>
        /// Справочник типов транспортировки
        /// </summary>
        public List<CatalogModel> TransportTypes { get; set; }
        /// <summary>
        /// Справочник 
        /// </summary>
        public List<CatalogModel> RailwayTransportTherms { get; set; }
        /// <summary>
        /// Справочник валют
        /// </summary>
        public List<CatalogModel> Currencies { get; set; }
        /// <summary>
        /// Справочник единиц измерения
        /// </summary>
        public List<CatalogModel> Units { get; set; }
        /// <summary>
        /// Название котировки
        /// </summary>
        public List<CatalogModel> DifferencialPriceText { get; set; }
        /// <summary>
        /// справочник типов котировок (за день, неделю, месяц)
        /// </summary>
        public List<CatalogModel> DifferencialPriceDateType { get; set; }
        /// <summary>
        /// Справочник типов значения котировки (минимальное, среднее, макисмальное за дату/промежуток)
        /// </summary>
        public List<CatalogModel> DifferencialPriceValueType { get; set; }

        /// <summary>
        /// Справочник типов значения котировки (минимальное, среднее, макисмальное за дату/промежуток)
        /// </summary>
        public List<CatalogModel> DifferencialPriceDateTypeDesc { get; set; }

        public CatalogModel DifferencialPriceDateTypeDescSelected { get; set; }
        public CatalogModel DifferencialPriceValueTypeSelected { get; set; }
        public CatalogModel DifferencialPriceTextSelected { get; set; }
        public CatalogModel DifferencialPriceDateTypeSelected { get; set; }

        /// <summary>
        /// Минимальное значение премии
        /// </summary>
        public decimal? DifferencialMin { get; set; }
        /// <summary>
        /// Максимальное значение премии
        /// </summary>
        public decimal? DifferencialMax { get; set; }

        public CatalogModel CurrencySelected { get; set; }
        public CatalogModel RailwayBeginSelected { get; set; }
        public CatalogModel RailwayEndSelected { get; set; }
        public CatalogModel SelectedUnit { get; set; }

        public CatalogModel TransportTypeSelected { get; set; }
        public CatalogModel TransportPointSelected { get; set; }
        //  public List<CatalogModel> IncothermsList { get; set; }
        public CatalogModel IncothermSelected { get; set; }

        public CatalogModel SelectedProduct { get; set; }
        public CatalogModel SelectedCountry { get; set; }

        public CatalogModel SelectedPlant { get; set; }
        public CatalogModel SelectedTradeType { get; set; }
        public CatalogModel SelectedNomenclature { get; set; }

        public CatalogModel SelectedQuality { get; set; }

        public CatalogModel SelectedPriceCurrency { get; set; }


        public decimal LotVolume { get; set; }
        public int LotsCount { get; set; }

        public decimal MinBetVolume { get; set; }
        public decimal MinStepVolume { get; set; }
        public decimal? PriceStep { get; set; }
        public decimal StartPrice { get; set; }
        public bool HasRedemptionPrice { get; set; }
        public decimal? RedemptionPrice { get; set; }
        /// <summary>
        /// Сумма за простой вагонов в день
        /// </summary>
        public int? TaxForUpTime { get; set; }

        /// <summary>
        /// с какого дня считается простой
        /// </summary>
        public int? DaysForUptime { get; set; }

        public DateTime DayToPay { get; set; }

        public string Contact { get; set; }

        public bool IsFixed
        {
            get
            {
                return SelectedTradeType?.Code == "closeFixed" || SelectedTradeType?.Code == "openFixed";
            }
        }

        public decimal TotalVolume { get; set; }

        //public decimal TotalVolume
        //{
        //    get
        //    {
        //        if (IsFixed)
        //        {
        //            return LotVolume * LotsCount;
        //        }
        //        return _totalVolume;

        //    }
        //    set
        //    {
        //        _totalVolume = value;
        //    }
        //}
        public bool IsAccepted { get; set; }
        public bool IsPreApproved { get; set; }
        //[JsonIgnore]
        public bool IsOffer { get; set; }
        public bool IsOrder { get; set; }

    }
    public class UserOrderViewModel
    {
        public List<SelectListItem> Contragent { get; set; }
        public int SelectedContragent { get; set; }
        public int OfferId { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
    }


    public class ResellBetViewModel
    {
        public int LotId { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }


    /// <summary>
    /// Модель для отображения информации о торгах
    /// </summary>
    public class TradeViewModel
    {
                /// <summary>
        /// Номер торгов, он же айди в БД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Айди бизнес-группы продавца
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// Одобрен ли торг
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        /// Айди продавца
        /// </summary>
        public int SellerId { get; set; }
        /// <summary>
        /// Наименование компании -продавца
        /// </summary>
        public string SellerName { get; set; }
        /// <summary>
        /// айди банковского счета, указанного как расчетный при создании торгов
        /// </summary>
        public int BankBillId { get; set; }
        /// <summary>
        /// Моделька отображения банковских реквизитов
        /// </summary>
        public BankBillViewModel BankBill { get; set; }
        /// <summary>
        /// Признак того, что торги фиксированне
        /// </summary>
        public bool IsFixed { get; set; }
        public bool IsActual { get; set; }
        public bool IsAuction { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DayToPay { get; set; }
        /// <summary>
        /// Наименование товара
        /// </summary>
        [StringLength(300)]
        public string ProductName { get; set; }
        /// <summary>
        /// Номенклдатура товра
        /// </summary>
        [StringLength(300)]
        public string ProductNomenclature { get; set; }
        [StringLength(300)]
        public string ProductQuality { get; set; }
        [StringLength(300)]
        public string ProductCountry { get; set; }
        [StringLength(300)]
        public string ProductManufacturer { get; set; }

        public int? ProductPassportId { get; set; }

        public decimal PriceStart { get; set; }
        public decimal? PriceStep { get; set; }
        public decimal? MaxPrice { get; set; }

        public decimal? TotalVolume { get; set; }
        public decimal? MinVolumeBet { get; set; }
        public decimal? MinVolumeStep { get; set; }
        public string TradeType { get; set; }
        public string Incotherms { get; set; }

        public string TankTherms { get; set; }

        public int? LotsCount { get; set; }
        public int? LotsCountAvailable { get; set; }
        public decimal? LotVolume { get; set; }

        public DateTime DeliveryDateFrom { get; set; }
        public DateTime DeliveryDateTo { get; set; }

        public virtual List<ContragentViewModel> Buyers { get; set; }
        public virtual List<BetViewModel> ActualBets { get; set; }
        [StringLength(300)]
        public string TransportType { get; set; }
        [StringLength(300)]
        public string ShipmentPoint { get; set; }
        [StringLength(300)]
        public string Unit { get; set; }
        public bool IsFuture { get; set; }
        public bool IsPast { get; set; }
        public bool IsOpened { get; set; }
        public string Filter { get; set; }
        public IEnumerable<int> AllContragentIds { get; set; }
        public List<int> CurrentUserContragentIds { get; set; }

        public bool IsPreapproved { get; set; }
        public bool IsAccepted { get; set; }
        public string ContactUser { get; set; }

        public string PriceCurrency { get; set; }
        /// <summary>
        /// Уточнение периода котировки
        /// </summary>
        public string DifferencialPriceDateTypeDesc { get; set; }

        /// <summary>
        /// Минимальное значение премии
        /// </summary>
        public decimal DifferencialMin { get; set; }
        /// <summary>
        /// Максимальное значение премии
        /// </summary>
        public decimal DifferencialMax { get; set; }
        /// <summary>
        /// Название котировки
        /// </summary>
        public string DifferencialPriceText { get; set; }
        /// <summary>
        /// Внятная формулировка ценообразования
        /// </summary>
        public string DifferencialPriceDesc { get; set; }
        /// <summary>
        /// Тип котировки (за день, неделю, месяц)
        /// </summary>
        public string DifferencialPriceDateType { get; set; }
        /// <summary>
        /// Тип значения котировки (минимальное, среднее, макисмальное за дату/промежуток)
        /// </summary>
        public string DifferencialPriceValueType { get; set; }
        /// <summary>
        /// Значение котировки (фактическая цена)
        /// </summary>
        public double DifferencialPriceValue { get; set; }

        public List<LotViewModel> Lots { get; set; }
        //  [Computed]
        public bool CanUserBet
        {
            get
            {
                return AllContragentIds.Intersect(CurrentUserContragentIds).Any();
            }
        }

        public bool IsOffer { get; set; }
        public bool IsOrder { get; set; }
        public List<OrderViewModel> Orders { get; set; }

        public int VolumeAvailable { get; set; }
    }

    public class TakePartModel
    {
        public List<SelectListItem> Contragent { get; set; }
        public List<string> SelectedContragents { get; set; }
        public int TradeId { get; set; }
        public List<int> CurrentContragentIds { get; set; }
    }

}
