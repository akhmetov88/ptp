using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("TradeBills")]
    public partial class TradeBill : BaseEntity
    {
        public TradeBill()
        {
            
        }
        public TradeBill(Trade trade, Contragent buyer, Contract contract, string addContractName, string billName)
        {
            AddContractName = addContractName;
            BillName = billName;
            Trade = trade;
            Buyer = buyer;
            Seller = trade.Seller;
            Contractt = contract;
            CreateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// �������� (�����) �����
        /// </summary>
        [StringLength(128)]
        public string BillName { get;set;}
  /// <summary>
  /// �������� (�����) ��������������� ��������
  /// </summary>
        [StringLength(128)]
        public string AddContractName { get; set; }
        /// <summary>
        /// ���� ������������ �������� ����������-��������
        /// </summary>
        public int ContracttId { get; set; }
        /// <summary>
        /// ��� �������
        /// </summary>
        public virtual Contract Contractt { get; set; }
        /// <summary>
        /// ���� �������� �������
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// ���� (����������)
        /// </summary>
        public int ToContragentId { get; set; }
        public virtual Contragent Buyer { get; set; }
        /// <summary>
        /// �� ���� (��������)
        /// </summary>
        public int FromContragentId { get; set; }
        public virtual Contragent Seller { get; set; }
        /// <summary>
        /// ���� ������
        /// </summary>
        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }

    }
}
