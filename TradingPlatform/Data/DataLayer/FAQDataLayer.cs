using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TradingPlatform.Data;

namespace TradingPlatform.Models.PageModel
{
    public class FAQDataLayer
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public HelpGroupViewModel GetHelpGroup(int id)
        {
            return FillHelpGroupViewModel(db.HelpGroups.FirstOrDefault(hg => hg.Id == id));
        }

        public List<HelpGroupViewModel> GetHelpGroups(int? id = null)
        {
            if (id.HasValue && id.Value > 0)
                return db.HelpGroups.Where(x => x.Id == id.Value).ToList().Select(hg => FillHelpGroupViewModel(hg)).OrderBy(h => h.OrderId).ToList();
            else
                return db.HelpGroups.ToList().Select(hg => FillHelpGroupViewModel(hg)).OrderBy(h => h.OrderId).ToList();
        }

        public List<SelectListItem> GetHelpGroupsDdl(int? selected = null)
        {
            if (!selected.HasValue)
                selected = 0;

            return GetHelpGroups().Select(x => new SelectListItem()
            {
                Text = string.Format("{0} (#{1})", x.TitleRu, x.Hashtag),
                Value = x.Id.ToString(),
                Selected = x.Id == selected,
            }).ToList();
        }

        public HelpViewModel GetHelp(int id)
        {
            return FillHelpViewModel(db.Helps.FirstOrDefault(h => h.Id == id));
        }

        public List<HelpViewModel> GetHelps(int? groupId = null)
        {
            if (groupId.HasValue)
                return db.Helps.Where(h => h.HelpGroupId == groupId.Value).ToList()
                    .Select(h => FillHelpViewModel(h)).OrderBy(h => h.OrderId).ToList();
            else
                return db.Helps.Select(h => FillHelpViewModel(h)).OrderBy(h => h.OrderId).ToList();
        }

        public int CalculateOrderId(int groupId)
        {
            return db.Helps.Where(x => x.HelpGroupId == groupId).Count() + 1;
        }

        public int CalculateGroupOrderId()
        {
            return db.HelpGroups.Count() + 1;
        }

        public HelpGroupViewModel FillHelpGroupViewModel(HelpGroup group)
        {
            if (group == null)
                return new HelpGroupViewModel();
            else
                return new HelpGroupViewModel()
                {
                    Id = group.Id,
                    OrderId = group.OrderId,
                    Hashtag = group.Hashtag,
                    Ua = group.Ua,
                    Ru = group.Ru,
                    En = group.En,
                    TitleRu = group.TitleRu,
                    TitleUa = group.TitleUa,
                    TitleEn = group.TitleEn,
                    UpdateDate = group.UpdateDate,
                    Helps = GetHelps(group.Id)
                };
        }

        public HelpViewModel FillHelpViewModel(Help help)
        {
            if (help == null)
                return new HelpViewModel();
            else
                return new HelpViewModel()
                {
                    Id = help.Id,
                    OrderId = help.OrderId,
                    Hashtag = help.Hashtag,
                    HelpGroupId = help.HelpGroupId,
                    Ua = help.Ua,
                    Ru = help.Ru,
                    En = help.En,
                    TitleRu = help.TitleRu,
                    TitleUa = help.TitleUa,
                    TitleEn = help.TitleEn,
                    UpdateDate = help.UpdateDate
                };
        }

        public void Create(HelpViewModel model)
        {
            if (db.Helps.Any(x => x.Hashtag == model.Hashtag.ToLower()))
                throw new Exception(string.Format("Хештег {0} уже существует в базе.", model.Hashtag));

            var item = db.Helps.Create();

            item.OrderId = model.OrderId;
            item.Hashtag = model.Hashtag;
            item.HelpGroupId = model.HelpGroupId;
            item.TitleRu = model.TitleRu;
            item.TitleUa = model.TitleUa;
            item.TitleEn = model.TitleEn;
            item.Ru = model.Ru;
            item.Ua = model.Ua;
            item.En = model.En;
            item.UpdateDate = DateTime.UtcNow;

            db.Helps.Add(item);
            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }

        public void Update(HelpViewModel model)
        {
            var item = db.Helps.SingleOrDefault(x => x.Id == model.Id);

            item.OrderId = model.OrderId;
            item.Hashtag = model.Hashtag;
            item.HelpGroupId = model.HelpGroupId;
            item.TitleRu = model.TitleRu;
            item.TitleUa = model.TitleUa;
            item.TitleEn = model.TitleEn;
            item.Ru = model.Ru;
            item.Ua = model.Ua;
            item.En = model.En;
            item.UpdateDate = DateTime.Now;

            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }

        public void Delete(int id)
        {
            var item = db.Helps.SingleOrDefault(x => x.Id == id);
            db.Helps.Remove(item);
            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }

        public void CreateGroup(HelpGroupViewModel model)
        {
            if (db.HelpGroups.Any(x => x.Hashtag == model.Hashtag.ToLower()))
                throw new Exception(string.Format("Хештег {0} уже существует в базе.", model.Hashtag));

            var item = db.HelpGroups.Create();

            item.OrderId = model.OrderId;
            item.Hashtag = model.Hashtag;
            item.TitleRu = model.TitleRu;
            item.TitleUa = model.TitleUa;
            item.TitleEn = model.TitleEn;
            item.Ru = model.Ru;
            item.Ua = model.Ua;
            item.En = model.En;
            item.UpdateDate = DateTime.UtcNow;

            db.HelpGroups.Add(item);
            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }

        public void UpdateGroup(HelpGroupViewModel model)
        {
            var item = db.HelpGroups.SingleOrDefault(x => x.Id == model.Id);

            item.OrderId = model.OrderId;
            item.Hashtag = model.Hashtag;
            item.TitleRu = model.TitleRu;
            item.TitleUa = model.TitleUa;
            item.TitleEn = model.TitleEn;
            item.Ru = model.Ru;
            item.Ua = model.Ua;
            item.En = model.En;
            item.UpdateDate = DateTime.Now;

            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }

        public void DeleteGroup(int id)
        {
            var item = db.HelpGroups.SingleOrDefault(x => x.Id == id);
            if (item.Helps.Count > 0)
                throw new Exception(string.Format("Не возможно удалить группу \"{0}\", так как у неё есть активные ответы ({1} шт.)", item.TitleRu, item.Helps.Count));

            db.HelpGroups.Remove(item);
            db.SaveChanges();
            StaticData.Inst.ReloadFAQ();
        }
    }
}
