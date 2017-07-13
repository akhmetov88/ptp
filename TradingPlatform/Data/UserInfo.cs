using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    [Table("UserInfo")]
    public partial class UserInfo : BaseEntity
    {
      

        [StringLength(20)]
        public string AdditionalPhone { get; set; }

        [StringLength(80)]
        public string AdditionalMail { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Patronymyc { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }
      
        public virtual ApplicationUser User { get; set; }
    }
}
