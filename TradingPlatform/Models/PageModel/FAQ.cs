using System;
using System.Collections.Generic;

namespace TradingPlatform.Models.PageModel
{
    public class HelpClass
    {
        public HelpClass()
        {
            Helps = new List<HelpClass>();
        }

        public string Hashtag { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }

        public List<HelpClass> Helps { get; set; }
    }

    public class HelpsViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public string Hashtag { get; set; }

        public string TitleRu { get; set; }
        public string TitleUa { get; set; }
        public string TitleEn { get; set; }

        public string Ru { get; set; }
        public string Ua { get; set; }
        public string En { get; set; }

        public DateTime UpdateDate { get; set; }
    }

    public class HelpGroupViewModel : HelpsViewModel
    {
        public List<HelpViewModel> Helps { get; set; }
    }

    public partial class HelpViewModel : HelpsViewModel
    {
        public int HelpGroupId { get; set; }
        public HelpGroupViewModel HelpGroup { get; set; }
    }
}
