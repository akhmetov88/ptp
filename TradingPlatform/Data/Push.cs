using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    [Table("Pushes")]
    public  class Push : BaseEntity
    {
        public Push()
        {
            
        }
        public Push(string device, string userId = null)
        {
            Device = "Chrome";
            DeviceId = device;
            UserId = string.IsNullOrEmpty(userId)?null:userId;
        }
        public string Device { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(2500)]
        [Index]
        public string DeviceId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class PushModel
    {
        public string User { get; set; }
        public string DeviceId { get; set; }
    }
}
