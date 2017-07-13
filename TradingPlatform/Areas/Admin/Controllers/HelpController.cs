using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TradingPlatform.Models;
using TradingPlatform.Models.PageModel;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class HelpController : Controller
    {
        private List<SelectListItem> _langs = new List<SelectListItem>();
        private List<SelectListItem> _pages = new List<SelectListItem>();
        private ContentDataLayer contentDataLayer = new ContentDataLayer();
        private ApplicationDbContext db = ApplicationDbContext.Create();
        [InLeftMenu]
        public ActionResult Index(string page = "help-main", string lang = "uk")
        {
            SetLangList(lang);
            ViewBag.LangList = _langs;

            SetPageList(page);
            ViewBag.PageList = _pages;

            ViewBag.Lang = lang;
            ViewBag.Page = page;

            return View(contentDataLayer.Get(page, lang));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(ContentView content, string page = "help-main", string lang = "uk")
        {
            SetLangList(lang);
            ViewBag.LangList = _langs;

            SetPageList(page);
            ViewBag.PageList = _pages;

            ViewBag.Lang = lang;
            ViewBag.Page = page;

            contentDataLayer.Set(content, page, lang, User.Identity.GetUserId());

            return View(content);
        }

        private void SetLangList(string lang)
        {
            _langs.Add(new SelectListItem() { Text = "Украинский", Value = "uk", Selected = lang == "uk" });
            _langs.Add(new SelectListItem() { Text = "Русский", Value = "ru", Selected = lang == "ru" });
        }
        private void SetPageList(string page)
        {

            foreach (var p in db.ContentPages.Where(c=>c.Alias.Contains("help")&&c.Lang.ToLower()=="uk").ToList())
            {
                if (_pages.All(c=>!c.Value.Equals(p.Alias)))
                {
                    _pages.Add(new SelectListItem() { Text = p.Descriptions, Value = p.Alias, Selected = page == p.Alias });
                }
            }
            //_page.Add(new SelectListItem() { Text = "Главная", Value = "help-main", Selected = page == "help-main" });
            //_page.Add(new SelectListItem() { Text = "Обратная связь", Value = "help-feedback", Selected = page == "help-feedback" });
            //_page.Add(new SelectListItem() { Text = "Создание торгов", Value = "help-tradecreate", Selected = page == "help-tradecreate" });
            //_page.Add(new SelectListItem() { Text = "Торги", Value = "help-trade", Selected = page == "help-trade" });
            //_page.Add(new SelectListItem() { Text = "Текущие торги", Value = "help-trade-current", Selected = page == "help-trade-current" });
            //_page.Add(new SelectListItem() { Text = "Будущие торги", Value = "help-trade-future", Selected = page == "help-trade-future" });
            //_page.Add(new SelectListItem() { Text = "Прошедшие торги", Value = "help-trade-past", Selected = page == "help-trade-past" });
            //_page.Add(new SelectListItem() { Text = "Торги продавца", Value = "help-sellertrades", Selected = page == "help-sellertrades" });
            //_page.Add(new SelectListItem() { Text = "Торги покупателя", Value = "help-buyertrades", Selected = page == "help-buyertrades" });
            //_page.Add(new SelectListItem() { Text = "История торгов", Value = "help-tradehistory", Selected = page == "help-tradehistory" });
            //_page.Add(new SelectListItem() { Text = "Авторизация", Value = "help-login", Selected = page == "help-login" });
            //_page.Add(new SelectListItem() { Text = "Регистрация", Value = "help-register", Selected = page == "help-register" });
            //_page.Add(new SelectListItem() { Text = "Сброс пароля", Value = "help-resetpass", Selected = page == "help-resetpass" }); 
            //_page.Add(new SelectListItem() { Text = "Забыл пароль", Value = "help-forgotpass", Selected = page == "help-forgotpass" });
            //_page.Add(new SelectListItem() { Text = "Смена паролья", Value = "help-changepass", Selected = page == "help-changepass" });
            //_page.Add(new SelectListItem() { Text = "Подтвеерждение сброса пароля", Value = "help-resetpassconfirm", Selected = page == "help-resetpassconfirm" });
            //_page.Add(new SelectListItem() { Text = "Профиль", Value = "help-editprofile", Selected = page == "help-editprofile" });
            //_page.Add(new SelectListItem() { Text = "Юридические лица ", Value = "help-account", Selected = page == "help-account" }); 
            //        _page.Add(new SelectListItem() { Text = "Создание юрлица", Value = "help-createcontragent", Selected = page == "help-createcontragent" });
            //_page.Add(new SelectListItem() { Text = "Всплывающее окно с  предупреждением не закрывать", Value = "help-noclose", Selected = page == "help-noclose" });



        }
    }
}