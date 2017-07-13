using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using NLog;
using TradingPlatform.Data;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Models.PageModel
{
    public class ContentDataLayer
    {
       // ApplicationDbContext db = new ApplicationDbContext();
        public ContentDataLayer()
        {
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ContentView Get(string alias, string lang = "")
        {
            using (var db = ApplicationDbContext.Create())
            {
                if (string.IsNullOrEmpty(alias))
                    return new ContentView() {Alias = alias, Lang = lang};

                if (string.IsNullOrEmpty(lang))
                    lang = LocalText.GetCultureTwoLetter();

                var cnt = db.ContentPages.Where(c => c.Alias == alias && c.Lang == lang).ToList()
                    .Select(c => new ContentView(c)).FirstOrDefault();
                if (cnt == null)
                {
                    cnt = new ContentView()
                    {
                        Alias = alias,
                        Lang = lang,
                        Title = alias,
                        KeyWords = alias,
                        Descriptions = alias,
                        HtmlContent = alias
                    };

                    db.Insert(new Content(cnt));
                }
                   // return new ContentView() { Alias = alias, Lang = lang, HtmlContent = "" };

                cnt.HtmlContent = HttpUtility.HtmlDecode(cnt.HtmlContent);
                return cnt; 
            }
        }
        

        public void Set(ContentView content, string alias, string lang = "", string userid=null)
        {
            using (var db = ApplicationDbContext.Create())
            {
                content.HtmlContent = HttpUtility.HtmlEncode(content.HtmlContent);
                var item = db.ContentPages.SingleOrDefault(x => x.Alias == alias && x.Lang == lang);
                if (item != null)
                {
                    item.Title = content.Title;
                    item.Descriptions = content.Descriptions;
                    item.KeyWords = content.KeyWords;
                    item.HtmlContent = content.HtmlContent;
                    item.UserId = userid;
                    item.UpdateDate = DateTime.UtcNow;
                }
                else
                {
                    item = db.ContentPages.Create();
                    item.Lang = content.Lang;
                    item.Alias = content.Alias;
                    item.Title = content.Title;
                    item.Descriptions = content.Descriptions;
                    item.KeyWords = content.KeyWords;
                    item.HtmlContent = content.HtmlContent;
                    item.UserId = userid;
                    item.UpdateDate = DateTime.UtcNow;
                    db.ContentPages.Add(item);

                }
                try
                {
                    logger.Info($"Content updatetd {alias} by {userid}");

                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            logger.Error(new Exception(String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));
                        }
                    }
                }
                catch (Exception exc)
                {
                   logger.Error(exc);
                } 
            }
        }
    }
}