using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TradingPlatform.Data;
using TradingPlatform.Models;
using TradingPlatform.Data.DataLayer;
using TradingPlatform.Extentions;
using TradingPlatform.Models.TradeModel;
using System.Collections.Generic;
using System;
using NLog;
using TradingPlatform.Models.OfferModels;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Enums;
using System.Threading;
using TradingPlatform.Messaging;

namespace TradingPlatform.Controllers
{
    [Authorize]
    public class OffersController : BaseController
    {
        #region VARS
        private static readonly NotificationHub hub = new NotificationHub();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ContragentDataLayer _dl;
        public ContragentDataLayer contragentDataLayer
        {
            get
            {
                return _dl ?? new ContragentDataLayer(Db);
            }
            private set
            {
                _dl = value;
            }
        }

        private TradeDataLayer _tdl;


        public TradeDataLayer tradeDataLayer
        {
            get
            {
                return _tdl ?? new TradeDataLayer(Db);
            }
            private set
            {
                _tdl = value;
            }
        }


        private ApplicationUserManager _userManager;

        private ApplicationDbContext _db;
        public OffersController()
        {

        }
        public OffersController(ApplicationUserManager userManager, ApplicationDbContext _db)
        {
            UserManager = userManager;
            Db = _db;
        }

        public ApplicationDbContext Db
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

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        #endregion
        // GET: Offers
        public ActionResult Index(bool actual = true)
        {
            var trades = Db.Trades;
            var user = UserManager.FindById(User.Identity.GetUserId());
            IEnumerable<TradeViewModel> model;
            if (actual)
                model = Db.Trades.ActualOffers().ToList().Select(c => tradeDataLayer.CreateTradeViewModel(c, user));
            else
                model = Db.Trades.NotActualOffers().ToList().Select(c => tradeDataLayer.CreateTradeViewModel(c, user));

            return View(model.ToList());
        }
        
      
        // GET: Offers/Create
        public async Task<ActionResult> Create()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // var user = db.Users.SingleOrDefault(c=>c.Id==User.Identity.GetUserId());
            if (user == null)
                throw new ArgumentNullException("user");
            if (user.UserContragents.Where(c => c.IsApproved).All(f => !f.IsSeller) || user.UserContragents.Count < 1)
            {
                ModelState.AddModelError("other", LocalText.Inst.Get("error", "contragentBuyerError", "Юридична особа має бути активована як продавець", "Юридическое лицо должно быть активировано как продавец"));
                return View("Error", ModelState);
            }
            var model = new NewAuctionViewModel(user, StaticData.Inst.GetCatalogModels());
          
            return View(model);
        }

        // POST: Offers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create(NewAuctionViewModel model)
        {
            model.IsOffer = true;
            model.IsOrder = false;
           // model.SelectedProduct = model?.ProductsList?.FirstOrDefault(c => c.Code == TradeTypes.closeFixed.ToString())??new Areas.Admin.Models.CatalogModel() { Code= "closeFixed" };
            try
            {
                if ((model.TradeEnd - model.TradeBegin).TotalMinutes < 59)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                responseText =
                                    LocalText.Inst.Get("error", "Offers.Create.WrongTimeOfOffer",
                                        "Час актуальності пропозиції повинен бути не меншим за годину",
                                        "Время актуальности предложения должно быть, как минимум, равным часу")
                            },
                            JsonRequestBehavior.AllowGet);
                }
                model.HasRedemptionPrice = true;
                model.IsPreApproved = true;
                model.IsAccepted = true;
                var trade = tradeDataLayer.CreateTrade(model, User.Identity.GetUserId());
                trade.BankBill = Db.BankBills.FirstOrDefault(c => c.Id == trade.BankBillId);
                //  var trademodel = tradeDataLayer.CreateTradeViewModel(trade);
                foreach (var user in UserManager.Users)
                {
                    EmailFactory.SendEmailAsync(new NewOffer(user, tradeDataLayer.CreateTradeViewModel(trade,user)));
                }

                await EmailFactory.Brodcast(new Broadcast() { Body = "Нова пропозиція", Subject = "На РТР оголошено нову пропозицію. Подавайте свої заявки" });
                
                //Mailer.SendMail("a.kuryanov@upklpg.com", "Новый торг на модерации",
                //    "Уважаемый модератор, просьба одобрить новый торг: " + Url.Action("TradesOnApproving", "Trade", null, Request.Url.Scheme));
                return Json(new { Success = true, redirectUrl = Url.Action("Index", "Offers") },
                    JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex);
                return Json(new { Success = false, responseText = LocalText.Inst.Get("error", "errorfilenotuploaded", "Паспорт товару не загружено, перейдіть на сторінку модерації торгів та спробуйте знову") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new { Success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Order(int offer)
        {
            try
            {
                var offerDb = Db.Trades.Find(offer);
                var user = UserManager.FindById(User.Identity.GetUserId());
                //    tradeDataLayer.CheckLotOnResell(user, lotDb);
                if(offerDb!=null)
                {
                    var model = new UserOrderViewModel()
                    {
                        Contragent = user.Group.Contragents.ToList().Select(v =>
                                    new SelectListItem()
                                    {
                                        Value = v.Id.ToString(),
                                        Text = v.LongName
                                    }).ToList(),
                        OfferId = offerDb.Id,
                        Price = offerDb.PriceStart,
                        Volume = (int)offerDb.TotalVolume
                    };
                    return PartialView("Partials/OrderModal", model);
                }
                  throw new ArgumentNullException("offer");
                
           
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return
                  Json(new
                  {
                      success = false,
                      error = $"{LocalText.Inst.Get("error", "errorBet", "Відбулася помилка", "Произошла ошибка")} {GetFullErrorMessage(ex)}"
                  },
                      JsonRequestBehavior.AllowGet);
            };
        }

        [HttpPost]
        public async Task<ActionResult> Order(UserOrderViewModel model)
        {
            try
            {
                var offer = Db.Trades.Find(model.OfferId);
                if (offer == null)
                {
                    logger.Info("Какой-то чмошник что-то чудит");
                    throw new ArgumentNullException("lot", "Bet must not be empty");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                                
                tradeDataLayer.CheckOfferOnOrder(user, model,offer);
                var order = new Order(model);
                Db.Insert(order, user.Id);
                offer.Buyers.Add(order.Buyer);
                Db.UpdateEntity(offer, user.Id);


                foreach (var u in offer.Orders.GroupBy(c => c.Buyer).SelectMany(f => f.Key.ContragentUsers).ToList())
                {
                    await hub.UpdateTradeTable(offer.Id, u.UserName);
                 
                }
                foreach (var u in offer.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(offer.Id, u.UserName);
                    EmailFactory.SendEmailAsync(new NewReplyToOffer(u, order));
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                logger.Error(ex);
                return
                 Json(new
                 {
                     success = false,
                     error = $"{LocalText.Inst.Get("error", "VolumeTooMuch", "Зменшіть об’єм Вашої заявки", "Уменьшите объем Вашей заявки")} {GetFullErrorMessage(ex)}"
                 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return
                 Json(new
                 {
                     success = false,
                     error = $"{LocalText.Inst.Get("error", "errorBet", "Відбулася помилка", "Произошла ошибка")} {GetFullErrorMessage(ex)}"
                 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AcceptOrder(int id)
        {
            try
            {
                var order = Db.Orders.Find(id);
                if (order == null)
                {
                    throw new ArgumentNullException("lotDb", "Lot must not be null");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (order.Volume > order.Trade.GetFreeOfferVolume())
                    throw new ArgumentException(LocalText.Inst.Get("error", "Offers.AcceptOfferDeniedByVolume", "Прийняти завяку неможливо, її об’єм більший за залишок", "Принять заявку невозможно, ее объём больше, чем остаток"));
                var orderViewModel = new OrderViewModel(order, user);
                return PartialView("Partials/AcceptOrderModal", orderViewModel);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", GetFullErrorMessage(ex));
                return PartialView("Error", ModelState);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AcceptOrder(OrderViewModel model)
        {
            try
            {

                var order = Db.Orders.Find(model.Id);
                if (order == null)
                {
                    logger.Info("Какой-то чмошник что-то чудит");
                    throw new ArgumentNullException("bet", "Bet must not be empty");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
#warning Insert validation logic
                // tradeDataLayer.CheckLotOnResell(user, lotdb);
                if (model.Volume > order.Trade.GetFreeOfferVolume())
                {
                    throw new ArgumentException(LocalText.Inst.Get("error", "Offers.AcceptOfferDeniedByVolume", "Прийняти завяку неможливо, її об’єм більший за залишок", "Принять заявку невозможно, ее объём больше, чем остаток"));
                }
                //order.IsAcceptedBySeller = true;
                //await Db.UpdateEntityAsync(order);
                await tradeDataLayer.OnRedemption(order.Trade, order, order.CreatedByUser);
                foreach (var u in order.Trade.Buyers.SelectMany(c => c.ContragentUsers).ToList())
                {
                    await hub.UpdateTradeTable(order.TradeId, u.UserName);
                }
                foreach (var u in order.Trade.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(order.TradeId, u.UserName);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return
                    Json(new
                    {
                        success = false,
                        error = $"{LocalText.Inst.Get("error", "errorBet", "Відбулася помилка", "Произошла ошибка")} {GetFullErrorMessage(ex)}"
                    }, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Offers/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Offer offer = await Db.Offers.FindAsync(id);
        //    if (offer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CreatedByUserId = new SelectList(Db.Users, "Id", "RegisterName", offer.CreatedByUserId);
        //    ViewBag.SellerId = new SelectList(Db.Contragents, "Id", "LongName", offer.SellerId);
        //    ViewBag.UpdatedByUserId = new SelectList(Db.Users, "Id", "RegisterName", offer.UpdatedByUserId);
        //    return View(offer);
        //}

        // POST: Offers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,SellerId,Price,Curency,Incotherms,Product,ProductQuality,VolumeStart,VolumeAvailable,ShipmentPoint,Comment,ActualTo,Created,Updated,CreatedByUserId,UpdatedByUserId")] Offer offer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Db.Entry(offer).State = EntityState.Modified;
        //        await Db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CreatedByUserId = new SelectList(Db.Users, "Id", "RegisterName", offer.CreatedByUserId);
        //    ViewBag.SellerId = new SelectList(Db.Contragents, "Id", "LongName", offer.SellerId);
        //    ViewBag.UpdatedByUserId = new SelectList(Db.Users, "Id", "RegisterName", offer.UpdatedByUserId);
        //    return View(offer);
        //}

        // GET: Offers/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Offer offer = await Db.Offers.FindAsync(id);
        //    if (offer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(offer);
        //}

        //// POST: Offers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Offer offer = await Db.Offers.FindAsync(id);
        //    Db.Offers.Remove(offer);
        //    await Db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
