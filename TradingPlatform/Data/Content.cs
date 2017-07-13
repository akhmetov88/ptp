using System;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contents")]
    public partial class Content : BaseEntity
    {

        public Content()
        {
            
        }


        public Content(ContentView model)
        {
            Id = model.Id;
            Alias = model.Alias??"";
            Lang = model.Lang??"";
            Title = model.Title??"";
            KeyWords = model.KeyWords??"";
            Descriptions = model.Descriptions??"";
            HtmlContent = model.HtmlContent??"";
        }

        [StringLength(7)]
        public string Lang { get; set; }

        [StringLength(256)]
        public string Title { get; set; }

        [StringLength(256)]
        public string KeyWords { get; set; }

        [StringLength(256)]
        public string Descriptions { get; set; }

        [StringLength(100)]
        public string Alias { get; set; }

        public string HtmlContent { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UserId { get; set; }
        //    public virtual ApplicationUser User { get; set; }
    }
}
