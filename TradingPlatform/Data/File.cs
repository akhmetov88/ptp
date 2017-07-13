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

//- статуту (або іншого установчого документу) юридичної особи,
//- протоколу загальних зборів про скасування всіх обмежень повноважень директора в разі їх наявності в статуті;
//- виписки з ЄДРПОУ;
//- витягу з ЄДРПОУ на день активації з максимально повною інформацією (отримати витяг можна в електронній формі на сайті Мін'юсту: https://usr.minjust.gov.ua/ з виділенням всіх галочок у формі заповнення інформації для вилучення);
//- свідоцтва ПДВ або витяг з реєстру платників ПДВ (у разі, якщо юридична особа, що підлягає реєстрації на РТР має статус платника ПДВ);
//- свідоцтва або витягу з реєстру платників єдиного податку (у разі, якщо юридична особа, що підлягає реєстрації на РТР має статус платника єдиного податку);
//- довідки банку про відкриття рахунку для кожного рахунку, який був зазначений Зареєстрованим Користувачем при заповнені заявки на реєстрацію юридичної особи;
