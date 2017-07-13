using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TradingPlatform.Models;
using PerpetuumSoft.Knockout;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Enums;
using PagedList;
using System.Net;
using TradingPlatform.Models.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class TokenController : Controller
    {
        private ApplicationDbContext _db;

        public ApplicationDbContext db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }
        // GET: Admin/Token
        [InLeftMenu]
        public ActionResult Index(int page = 1, int? count = null, string search = "")
        {
            search = search.ToLower().Trim();
            ViewBag.Search = search;
            var result = LocalText.Tokens;
            if(!string.IsNullOrEmpty(search))
                result = result.Where(x => x.Key.ToLower().Contains(search) || x.Ru.ToLower().Contains(search) || x.Uk.ToLower().Contains(search)).ToList();

            return View(result.OrderBy(x => x.Key).ToPagedList(page, 30));
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Edit(Token model, FormCollection col)
        {
            if (ModelState.IsValid)
            {
                return Json(new {
                    Success = LocalText.Update(model.Id, model.Key, model.Uk, model.Ru, User.Identity.GetUserId()),
                    Id = model.Id,
                    Message = LocalText.Inst.Get("error", "EditTokenDone","")
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {
                    Success = false,
                    Id = model.Id,
                    Message = LocalText.Inst.Get("error", "ErrorEditToken")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public string Test(string id)
        {
            return UpdateTokens(id);
        }
        public string UpdateContents(string instance)
        {
            if (String.IsNullOrEmpty(instance))
                return "notfound";
            try
            {
                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 } )
                {
                    var data = wc.DownloadString($"https://{instance}/api/contents");
                    var models = JsonConvert.DeserializeObject<IEnumerable<ContentView>>(data);
                    List<string> updates = new List<string>();
                    foreach (var model in models)
                    {
                        var localvalue = db.ContentPages.Find(model.Id);
                        if (localvalue.UpdateDate < model.Updated && localvalue.Alias == model.Alias)
                        {
                            localvalue.Updated = model.Updated.Value;
                            localvalue.HtmlContent = model.HtmlContent;
                            localvalue.KeyWords = model.KeyWords;
                            localvalue.Title = model.Title;
                            localvalue.Descriptions = model.Descriptions;
                            db.UpdateEntity(localvalue, User.Identity.GetUserId());
                            updates.Add(localvalue.Alias);
                        }
                        else if (localvalue == null)
                        {
                            db.Insert(new Content(model), User.Identity.GetUserId());
                        }
                    }
                    return $"You have a {updates.Count()} {String.Join("; ", updates)}";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateCatalogs(string instance)
        {
            if (String.IsNullOrEmpty(instance))
                return "notfound";
            try
            {
                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var data = wc.DownloadString($"https://{instance}/api/catalogs");
                    var models = JsonConvert.DeserializeObject<IEnumerable<CatalogModel>>(data);
                    List<string> updates = new List<string>();
                    foreach (var model in models)
                    {
                        var localvalue = db.Catalogs.Find(model.Id);
                        if (localvalue != null && localvalue.Updated < model.Updated && localvalue.Code == model.Code)
                        {
                            localvalue.Updated = model.Updated;
                            localvalue.DescRu = model.DescRu;
                            localvalue.DescUk = model.DescUk;
                            localvalue.Ru = model.DescRu;
                            localvalue.Uk = model.uk;
                            db.UpdateEntity(localvalue, User.Identity.GetUserId());
                            updates.Add(localvalue.Code);
                        }
                        else if (localvalue == null)
                        {
                            var newcatalog = new Catalog(model);
                            db.Insert(newcatalog);
                        }
                    }
                    return $"You have a {updates.Count()} {String.Join("; ", updates)}";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public string UpdateTokens(string instance)
        {
            if (String.IsNullOrEmpty(instance))
                return "notfound";
            try
            {
                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var data = wc.DownloadString($"https://{instance}/api/tokens");
                    var models = JsonConvert.DeserializeObject<IEnumerable<Token>>(data);
                    List<string> updates = new List<string>();

                    foreach (var model in models)
                    {
                        var localvalue = db.Tokens.Find(model.Id);
                        if (localvalue != null)
                        {
                            if (localvalue.Updated < model.Updated && localvalue.Key == model.Key)
                            {
                                localvalue.Updated = model.Updated;
                                localvalue.PageLink = model.PageLink;
                                localvalue.Uk = model.Uk;
                                db.UpdateEntity(localvalue, User.Identity.GetUserId());
                                updates.Add(localvalue.Key);
                            }
                        }
                        else
                        {
                            var newtoken = new Token
                            {
                                Key = model.Key,
                                PageLink = model.PageLink,
                                Ru = model.Ru,
                                Uk = model.Uk,
                                Updated = model.Updated
                            };
                            db.Insert(newtoken, User.Identity.GetUserId());
                        }
                        
                         
                        
                    }
                    return $"You have a {updates.Count()} {String.Join("; ", updates)}";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}