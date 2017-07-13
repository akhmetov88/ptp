using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Trades")]
    public partial class Trade : BaseEntity
    {
        public Trade()
        {
            Buyers = new List<Contragent>();
            Bets = new HashSet<Bet>();
            Lots = new HashSet<Lot>();
            News = new HashSet<New>();
        }


        public int SellerId { get; set; }
        public virtual Contragent Seller { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? IsFinallyApproved
        {
            get { return IsAccepted && IsPreapproved; }
            set { }
        }
        public int BankBillId { get; set; }
        public virtual BankBill BankBill { get; set; }
        public bool IsPreapproved { get; set; }
        public bool? IsFixed { get; set; }
        [StringLength(128)]
        public string Type { get; set; }
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
        /// Период котировки (за день, неделю, месяц)
        /// </summary>
        public string DifferencialPriceDateType { get; set; }

        /// <summary>
        /// Уточнение периода котировки
        /// </summary>
        public string DifferencialPriceDateTypeDesc { get; set; }


        /// <summary>
        /// Тип значения котировки (минимальное, среднее, макисмальное за дату/промежуток)
        /// </summary>
        public string DifferencialPriceValueType { get; set; }
        /// <summary>
        /// Значение котировки (фактическая цена)
        /// </summary>
        public double DifferencialPriceValue { get; set; }


        /// <summary>
        /// Сумма за простой вагонов в день
        /// </summary>
        public int TaxForUpTime { get; set; }
        /// <summary>
        /// Валюта расчета простоев
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// с какого дня считается простой
        /// </summary>
        public int DaysForUptime { get; set; }
        /// <summary>
        /// Открыт ли торг для ставок
        /// </summary>
        public bool IsOpened { get; set; }

        public bool IsClosedByBills { get; set; }
        /// <summary>
        /// Валюта расчета цены
        /// </summary>
        public string PriceCurrency { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? IsActual
        {
            get
            {
                return (DateBegin <= DateTime.UtcNow && DateEnd >= DateTime.UtcNow) && IsOpened && IsFinallyApproved.Value && Lots.Any(c => c.IsActual)
             || ((IsOffer || IsOrder) && DateEnd > DateTime.UtcNow && (TotalVolume.Value - Orders?.Where(o => o.IsAcceptedBySeller).Select(c => c.Volume).DefaultIfEmpty(0).Sum() > 0));
            }
            set { }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? AllowBets
        {
            get
            //{ return LotsCount >= Bets.Where(c => c.IsActual || c.IsRedemption).ToList().Sum(c => c.LotsCount) && LotsCountAvailable.Value > 0 && IsActual.Value;}
            { return Lots.Any(c => c.IsActual) && !Lots.All(f => f.IsSelled) && IsActual.Value; }
            set { }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? IsSuccefullyClosed
        {
            get
            {
                if ((IsPast != null && IsPast.Value) || (!IsOpened))
                {
#warning Убрал ограничение по поводу одного победителя
                    return (Buyers.ToList().Select(c => c.GroupId).Distinct().Count() > 0);
                }
                return false; //!IsActual.Value&& (Buyers.ToList().Select(c => c.GroupId).Distinct().Count() > 1);
            }
            set { }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? IsFuture
        {
            get { return DateBegin >= DateTime.UtcNow; }
            set { }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? IsPast
        {
            get
            {
                if ((IsOpened && IsFuture.Value))
                {
                    return false;
                }
                return (DateEnd <= DateTime.UtcNow) || !IsActual.Value;
            }
            set { }
        }
        /// <summary>
        /// Разослано ли уведомление о начале торгов
        /// </summary>
        public bool IsProcessed { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DateBegin { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DateEnd { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DayToPay { get; set; }
        [StringLength(300)]
        public string ProductName { get; set; }
        [StringLength(300)]
        public string ProductNomenclature { get; set; }
        [StringLength(300)]
        public string ProductQuality { get; set; }
        [StringLength(300)]
        public string ProductCountry { get; set; }
        [StringLength(300)]
        public string ProductManufacturer { get; set; }

        public virtual File ProductPassport { get; set; }

        public string TankTherms { get; set; }

        public string Contact { get; set; }
        public string Incothermns { get; set; }

        public decimal PriceStart { get; set; }
        public decimal? PriceStep { get; set; }
        public decimal? MaxPrice { get; set; }

        public decimal? TotalVolume { get; set; }
        public decimal? MinVolumeBet { get; set; }
        public decimal? MinVolumeStep { get; set; }

        public int? LotsCount { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? LotsCountAvailable
        {
            get { return Lots.Count(c => c.IsActual && !c.IsSelled); }
            set { }
        }
        public decimal? LotVolume { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDateFrom { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDateTo { get; set; }

        public string AcceptedByUserId { get; set; }
        public virtual ApplicationUser AcceptedByUser { get; set; }

        public string ApprovedByUserId { get; set; }
        public virtual ApplicationUser ApprovedByUser { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? ApprovedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? AcceptedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EditedDate { get; set; }
        public virtual ICollection<Contragent> Buyers { get; set; }
        public virtual ICollection<Bet> Bets { get; set; }
        [StringLength(300)]
        public string TransportType { get; set; }
        [StringLength(300)]
        public string ShipmentPoint { get; set; }
        [StringLength(300)]
        public string Unit { get; set; }
        [StringLength(300)]
        public string RailwayBegin { get; set; }
        [StringLength(300)]
        public string RailwayEnd { get; set; }

        //public string UpdateByUserId { get; set; }
        //public virtual ApplicationUser UpdateByUser { get; set; }

        public string FirstTherms { get; set; }

        public bool IsAccepted { get; set; }

        public virtual ICollection<Lot> Lots { get; set; }

        public bool IsOffer { get; set; }
        public bool IsOrder { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<New> News { get; set; }

    }
}
