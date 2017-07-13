using System;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Notifications")]
    public partial class Notification : BaseEntity
    {
     
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Email { get; set; }

        public bool IsViewed { get; set; }
        
        public DateTime CreateDate { get; set; }
        public DateTime ViewedDate { get; set; }

        public string FromUserId { get; set; }
        public virtual ApplicationUser FromUser { get; set; }

        public string ToUserId { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
    }
}
