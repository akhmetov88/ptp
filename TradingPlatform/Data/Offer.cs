using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;


namespace TradingPlatform.Data
{

    [Table("Offers")]
    public partial class Offer : BaseEntity
    {
        public Offer()
        {

        }
      

        public int SellerId { get; set; }
        public virtual Contragent Seller { get; set; }
        public string Unit { get; set; }
        public string TransportType { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// Валюта предложения, смотрим справочник
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Инкотермс, смотрим справочник
        /// </summary>
        public string Incotherms { get; set; }
        /// <summary>
        /// Наименование продукта, смотрим справочник
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// Качество продукта, смотрим справочник
        /// </summary>
        public string ProductQuality { get; set; }
        public int VolumeStart { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public int VolumeAvailable
        //{
        //    get
        //    {
        //        return Orders.Any() ? VolumeStart - Orders.Where(c => c.IsAcceptedBySeller).Sum(f => f.Volume) : VolumeStart;
        //    }
        //    set { }
        //}
        public string ShipmentPoint { get; set; }
        public string Comment { get; set; }
        public DateTime ActualTo { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        //[NotMapped, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public bool IsActual
        //{
        //    get
        //    {
        //        return 
        //    }
        //    set { }
        //}
    }
}
