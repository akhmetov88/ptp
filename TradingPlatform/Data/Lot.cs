
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace TradingPlatform.Data
{
    [Table("Lots")]
    public partial class Lot : BaseEntity
    {
        public Lot()
        {
            Bets = new HashSet<Bet>();
        }
        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }
        public int LotNumber { get; set; }
        public int? BuyerId { get; set; }
        public virtual Contragent Buyer { get; set; }
        public int SellerId { get; set; }
        public virtual Contragent Seller { get; set; }
        public decimal Volume { get; set; }
        /// <summary>
        /// ������� ���� ���� (����, �������, ���������� � MinPrice
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// ����������� ������ ������
        /// </summary>
        public decimal MinPrice { get; set; }
        public bool IsActual { get; set; }
        public bool IsSelled { get; set; }
        /// <summary>
        /// ����� ��������� �������� ��������/����������
        /// </summary>
        public DateTime ElapsingTime { get; set; }
        /// <summary>
        /// �������� ����, ��� �������� ����������, ��� ������ � �����. �� ���������� ������� ElapsingTime ���������� switch ����� ��������
        /// </summary>
        public bool OnThinking { get; set; }
        /// <summary>
        /// C������ ���������������
        /// </summary>
        public int ReSellingCount { get; set; }
        public virtual ICollection<Bet> Bets { get; set; } 

    }
}
