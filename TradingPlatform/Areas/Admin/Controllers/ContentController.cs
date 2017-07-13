using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TradingPlatform.Messaging;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Models.PageModel;
using TradingPlatform.Models.ViewModel;


namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class ContentController : Controller
    {
        private List<SelectListItem> _lang = new List<SelectListItem>();
        private List<SelectListItem> _page = new List<SelectListItem>();
        private ContentDataLayer contentDataLayer = new ContentDataLayer();

       
        [InLeftMenu]
        public ActionResult Index(string page = "main", string lang = "uk")
        {
            SetLangList(lang);
            ViewBag.LangList = _lang;

            SetPageList(page);
            ViewBag.PageList = _page;

            ViewBag.Lang = lang;
            ViewBag.Page = page;
            
            return View(contentDataLayer.Get(page, lang));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(ContentView content, string page = "main", string lang = "uk")
        {
            SetLangList(lang);
            ViewBag.LangList = _lang;

            SetPageList(page);
            ViewBag.PageList = _page;

            ViewBag.Lang = lang;
            ViewBag.Page = page;

            contentDataLayer.Set(content, page, lang);

            return View(content);
        }
        
        private void SetLangList(string lang)
        {

            _lang.Add(new SelectListItem() { Text = "Украинский", Value = "uk", Selected = lang == "uk" });
            _lang.Add(new SelectListItem() { Text = "Русский", Value = "ru", Selected = lang == "ru" });
        }
        private void SetPageList(string page)
        {
            _page.Add(new SelectListItem() { Text = "Главная", Value = "main", Selected = page == "main" });
            _page.Add(new SelectListItem() { Text = "Правила аккредитации", Value = "accreditation", Selected = page == "accreditation" });
            _page.Add(new SelectListItem() { Text = "Как пользоваться ЭТП", Value = "howituse", Selected = page == "howituse" });
            _page.Add(new SelectListItem() { Text = "О нас", Value = "about", Selected = page == "about" });
            _page.Add(new SelectListItem() { Text = "Правила и инструкции", Value = "rules", Selected = page == "rules" });
            _page.Add(new SelectListItem() { Text = "Политика конфиденциальности", Value = "privacypolicy", Selected = page == "privacypolicy" });
            _page.Add(new SelectListItem() { Text = "FAQ", Value = "faq", Selected = page == "faq" });
            _page.Add(new SelectListItem() { Text = "Документы", Value = "documents", Selected = page == "documents" });
            _page.Add(new SelectListItem() { Text = "Условия использования", Value = "therms", Selected = page == "therms" });
            _page.Add(new SelectListItem() { Text = "Оферта", Value = "offer", Selected = page == "offer" });
            _page.Add(new SelectListItem() { Text = "Благодарность за регистрацию", Value = "registerthanks", Selected = page == "registerthanks" });
            _page.Add(new SelectListItem() { Text = "Благодарность за регистрацию Юрлица", Value = "registerthanksjur", Selected = page == "registerthanksjur" });
            _page.Add(new SelectListItem() { Text = "Благодарность за изменение инфы юрлицом", Value = "editthanksjur", Selected = page == "editterthanksjur" });
            _page.Add(new SelectListItem() { Text = "Просьба подтвердить почту", Value = "confirmemail", Selected = page == "confirmemail" });
            _page.Add(new SelectListItem() { Text = "Договор услуг", Value = "contractService", Selected = page == "contractService" });
            _page.Add(new SelectListItem() { Text = "Договор продаж", Value = "contractProduct", Selected = page == "contractProduct" }); 
            _page.Add(new SelectListItem() { Text = "Дополнительный договор", Value = "addcontractProduct", Selected = page == "addcontractProduct" });

            _page.Add(new SelectListItem() { Text = "Инфо после созания торгов", Value = "tradethanks", Selected = page == "tradethanks" });
        }
    }
}