using System;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tokens")]
    public partial class Token:BaseEntity
    {
        public Token() { }

        public Token(TokenModel token)
        {
            Id = token.Id;
            Key = token.Key??"";
            Uk = token.Uk??"";
            Ru = token.Ru??"";
            PageLink = token.PageLink??"";
            UpdateDate = token.UpdateDate??DateTime.UtcNow;
        }
        [Required, StringLength(100)]
        public string Key { get; set; }

        public string Uk { get; set; }

        public string Ru { get; set; }

        public string PageLink { get; set; }

        public DateTime? UpdateDate { get; set; }
        
        public string UserId { get; set; }
      //  public virtual ApplicationUser User { get; set; }
    }
}