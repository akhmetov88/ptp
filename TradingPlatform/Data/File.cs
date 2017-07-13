using TradingPlatform.Models;
using System;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Files")]
    public partial class File : BaseEntity
    {
       
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public bool IsApproved { get; set; }
        [StringLength(255)]
        public string Comment { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ContragentId { get; set; }
        public virtual Contragent Contragent { get; set; }

       // public string Link { get; set; }

        [StringLength(128)]
        public string ApprovedByUserId { get; set; }
        public virtual ApplicationUser ApprovedByUser { get; set; }

        public int? FileTypeId { get; set; }
        public virtual FileType FileType { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}

//- ������� (��� ������ ����������� ���������) �������� �����,
//- ��������� ��������� ����� ��� ���������� ��� �������� ����������� ��������� � ��� �� �������� � ������;
//- ������� � ������;
//- ������ � ������ �� ���� ��������� � ����������� ������ ����������� (�������� ����� ����� � ���������� ���� �� ���� ̳�'����: https://usr.minjust.gov.ua/ � ��������� ��� ������� � ���� ���������� ���������� ��� ���������);
//- �������� ��� ��� ����� � ������ �������� ��� (� ���, ���� �������� �����, �� ������ ��������� �� ��� �� ������ �������� ���);
//- �������� ��� ������ � ������ �������� ������� ������� (� ���, ���� �������� �����, �� ������ ��������� �� ��� �� ������ �������� ������� �������);
//- ������ ����� ��� �������� ������� ��� ������� �������, ���� ��� ���������� ������������� ������������ ��� �������� ������ �� ��������� �������� �����;
