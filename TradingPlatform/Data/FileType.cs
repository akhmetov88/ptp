namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FileTypes")]
    public partial class FileType :BaseEntity
    {


        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Desc { get; set; }

        public string Alias { get; set; }
      
        //public string CreateByUserId { get; set; }
        //public virtual AspNetUser CreateByUser { get; set; }

    //    public virtual ICollection<File> Files { get; set; }


    }
}

//- ������� (��� ������ ����������� ���������) �������� �����,
//- ��������� ��������� ����� ��� ���������� ��� �������� ����������� ��������� � ��� �� �������� � ������;
//- ������� � ������;
//- ������ � ������ �� ���� ��������� � ����������� ������ ����������� (�������� ����� ����� � ���������� ���� �� ���� ̳�'����: https://usr.minjust.gov.ua/ � ��������� ��� ������� � ���� ���������� ���������� ��� ���������);
//- �������� ��� ��� ����� � ������ �������� ��� (� ���, ���� �������� �����, �� ������ ��������� �� ��� �� ������ �������� ���);
//- �������� ��� ������ � ������ �������� ������� ������� (� ���, ���� �������� �����, �� ������ ��������� �� ��� �� ������ �������� ������� �������);
//- ������ ����� ��� �������� ������� ��� ������� �������, ���� ��� ���������� ������������� ������������ ��� �������� ������ �� ��������� �������� �����;
