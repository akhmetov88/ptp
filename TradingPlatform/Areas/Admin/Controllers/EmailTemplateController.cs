using System.Collections.Generic;
using System.Web.Mvc;
using TradingPlatform.Models;
using TradingPlatform.Models.PageModel;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class EmailTemplateController : Controller
    {
        private List<SelectListItem> _lang = new List<SelectListItem>();
        private List<SelectListItem> _page = new List<SelectListItem>();
        private ContentDataLayer contentDataLayer = new ContentDataLayer();

        [InLeftMenu]
        public ActionResult Index(string page = "registration", string lang = "uk")
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
        public ActionResult Index(ContentView content, string page = "registration", string lang = "uk")
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
            //using (var db = new DataEntities())
            //{
            //    List<SelectListItem> pagelist =
            //        db.ContentPages.Select(c => new SelectListItem() {Value = c.Alias, Text = c.Descriptions}).ToList();
            //}
            
            _page.Add(new SelectListItem() { Text = "Регистрация", Value = "registration", Selected = page == "registration" });
            _page.Add(new SelectListItem() { Text = "Регистрация юридического лица", Value = "registration_legal", Selected = page == "registration_legal" });
            _page.Add(new SelectListItem() { Text = "Юридическое лицо активировано", Value = "legal_active", Selected = page == "legal_active" });
            _page.Add(new SelectListItem() { Text = "Юридическое лицо не активировано", Value = "legal_not_active", Selected = page == "legal_not_active" });
            _page.Add(new SelectListItem() { Text = "Оповещение объявления новых торгов", Value = "new_trade", Selected = page == "new_trade" });
            _page.Add(new SelectListItem() { Text = "Начались торги", Value = "start_trad", Selected = page == "start_trad" });
            _page.Add(new SelectListItem() { Text = "Изменение данных", Value = "change_data", Selected = page == "change_data" });
            _page.Add(new SelectListItem() { Text = "Сделана ставка (изменяемый объем лота)", Value = "rate_variable_volume", Selected = page == "rate_variable_volume" });
            _page.Add(new SelectListItem() { Text = "Сделана ставка (фиксированный объем лота)", Value = "rate_fix_volume", Selected = page == "rate_fix_volume" });
            _page.Add(new SelectListItem() { Text = "Ставка перебита", Value = "bet_slaughtered", Selected = page == "bet_slaughtered" });
            _page.Add(new SelectListItem() { Text = "Выкупили (изменяемый объем лота)", Value = "bought_variable_volume", Selected = page == "bought_variable_volume" });
            _page.Add(new SelectListItem() { Text = "Выкупили (фиксированный объем лота)", Value = "bought_fix_volume", Selected = page == "bought_fix_volume" });
            _page.Add(new SelectListItem() { Text = "Ставка выиграла (изменяемый объем лота)", Value = "won_variable_volume", Selected = page == "won_variable_volume" });
            _page.Add(new SelectListItem() { Text = "Ставка выиграла (фиксированный объем лота)", Value = "won_fix_volume", Selected = page == "won_fix_volume" });
            _page.Add(new SelectListItem() { Text = "Снятие аккредитации (блокировка)", Value = "lock_accreditation", Selected = page == "lock_accreditation" });
            _page.Add(new SelectListItem() { Text = "Продление торгов", Value = "extending_trading", Selected = page == "extending_trading" });
            _page.Add(new SelectListItem() { Text = "Завершились торги", Value = "complete_trad", Selected = page == "complete_trad" });
            _page.Add(new SelectListItem() { Text = "Ни одна ставка не выиграла", Value = "loserLetter", Selected = page == "loserLetter" });
            _page.Add(new SelectListItem() { Text = "Письмо продавцу о заключенных сделках", Value = "sellerLetter", Selected = page == "sellerLetter" });
            _page.Add(new SelectListItem() { Text = "Письмо об изменении информации контрагентом", Value = "juridicalInfoLetter", Selected = page == "juridicalInfoLetter" });
            _page.Add(new SelectListItem() { Text = "Продавцу о выкупленной ставке", Value = "redemptionBet", Selected = page == "redemptionBet" });
            _page.Add(new SelectListItem() { Text = "Контрагенту о блокировке в связи с изменением инфы", Value = "changeInfoMail", Selected = page == "changeInfoMail" });
            _page.Add(new SelectListItem() { Text = "Умышленно перебитая ставка ", Value = "rebetterMail", Selected = page == "rebetterMail" });
            _page.Add(new SelectListItem() { Text = "Емейл о предварительной модерации торгов", Value = "preapproveTrade", Selected = page == "preapproveTrade" });
            _page.Add(new SelectListItem() { Text = "Емейл о полной модерации торгов ", Value = "approveTrade", Selected = page == "approveTrade" });
        
    }
    }
}