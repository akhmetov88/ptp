using System.Collections.Generic;

namespace TradingPlatform.Data
{

    public partial class Config : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
