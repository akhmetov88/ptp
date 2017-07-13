using System;
using TradingPlatform.Models.ProfileModel;

namespace TradingPlatform.Models.EntityModel
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public string UserId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public UserViewModel User { get; set; }
        public ContragentViewModel Contragent { get; set; }
        public UserViewModel ApprovedByUser { get; set; }
        public FileTypeViewModel FileType { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class FileTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Alias { get; set; }
    }
}
