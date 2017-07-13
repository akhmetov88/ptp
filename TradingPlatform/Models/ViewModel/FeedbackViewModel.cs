using System.ComponentModel.DataAnnotations;

namespace TradingPlatform.Models.ViewModel
{
    public class FeedbackViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Text { get; set; }

    }
}
