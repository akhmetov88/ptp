using System.Collections.Generic;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Groups")]
    public partial class Group:BaseEntity
    {      
        public  string GroupName { get; set; }
        public virtual ICollection<Contragent> Contragents { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
     
    }
}
