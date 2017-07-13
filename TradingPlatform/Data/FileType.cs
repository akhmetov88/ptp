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

//- статуту (або іншого установчого документу) юридичної особи,
//- протоколу загальних зборів про скасування всіх обмежень повноважень директора в разі їх наявності в статуті;
//- виписки з ЄДРПОУ;
//- витягу з ЄДРПОУ на день активації з максимально повною інформацією (отримати витяг можна в електронній формі на сайті Мін'юсту: https://usr.minjust.gov.ua/ з виділенням всіх галочок у формі заповнення інформації для вилучення);
//- свідоцтва ПДВ або витяг з реєстру платників ПДВ (у разі, якщо юридична особа, що підлягає реєстрації на РТР має статус платника ПДВ);
//- свідоцтва або витягу з реєстру платників єдиного податку (у разі, якщо юридична особа, що підлягає реєстрації на РТР має статус платника єдиного податку);
//- довідки банку про відкриття рахунку для кожного рахунку, який був зазначений Зареєстрованим Користувачем при заповнені заявки на реєстрацію юридичної особи;
