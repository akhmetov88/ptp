using System.Collections.Generic;
using System.Web;

namespace TradingPlatform.Models
{
    public class NonResidentViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set;}       
        
        public ICollection<HttpPostedFileBase> Files { get; set; }
    }
}