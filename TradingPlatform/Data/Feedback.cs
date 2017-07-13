using System;
using System.ComponentModel.DataAnnotations;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    public partial class Feedback : BaseEntity
    {
  
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Email { get; set; }
        [StringLength(25)]
        public string PhoneNumber { get; set; }
        [StringLength(350)]
        public string Subject { get; set; }
        public string Text { get; set; }

        public DateTime Date { get; set; }
        public bool IsCommited { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
