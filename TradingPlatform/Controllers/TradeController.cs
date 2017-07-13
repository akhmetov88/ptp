using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using TradingPlatform.Data;
using TradingPlatform.Data.DataLayer;
using TradingPlatform.Enums;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.TradeModel;
using TradingPlatform.Models.NotifyModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingPlatform.Messaging;

namespace TradingPlatform.Controllers
{
    [Authorize]
    public class TradeController : BaseController
    {
        // private static readonly TradeController _instance = new TradeController();
        private static readonly NotificationHub hub = new NotificationHub();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region VARS
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

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private PtpRolesManager _rolesManager;
        private ApplicationDbContext _db;
        public TradeController()
        {

        }


        public TradeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,
            PtpRolesManager rolesManager, ApplicationDbContext db)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RolesManager = rolesManager;
            Db = db;
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
        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public PtpRolesManager RolesManager
        {
            get { return _rolesManager ?? HttpContext.GetOwinContext().Get<PtpRolesManager>(); }
            private set { _rolesManager = value; }
        }

        #endregion

        public class RolesAttribute : AuthorizeAttribute
        {
            public RolesAttribute(params string[] roles)
            {
                Roles = String.Join(",", roles);
            }
        }


      
        [MyAuthorize(Roles = "trader, byer, watcher")]
        [HttpGet, InLeftMenu]
        public async Task<ActionResult> Index(string filter = null, int count = 10)
        {
            ViewBag.MenuAnchor = "trade";
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var trades = Db.TradesApproved(count)
                .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser, filter))
                .ToList();
            foreach (var trade in Db.TradesApproved().ToList().Where(c => c.IsActual.Value).ToList())
            {
                if (trade.Type != TradeTypes.quotation.ToString())
                {
                    await tradeDataLayer.CheckBets(trade);
                }
                else if(!trade.IsOffer&&!trade.IsOrder)
                {
                    foreach (var lot in trade.Lots)
                    {
                        await tradeDataLayer.CheckTradeLot(lot);

                    }
                }
            }
            ViewBag.Count = count;
            if (!String.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "actual":
                        trades = trades.Where(c => c.IsActual).ToList();
                        ViewBag.Count = trades.Count;
                        break;
                    case "future":
                        trades = trades.Where(c => c.IsFuture).ToList();
                        ViewBag.Count = trades.Count;
                        break;
                    case "past":
                        trades = trades.Where(c => c.IsPast).ToList();
                        ViewBag.Count = trades.Count;
                        break;
                    default:
                        break;
                }
            }
            return View(trades);
        }

        [MyAuthorize(Roles = "trader, byer, watcher")]
        [HttpGet, InLeftMenu]
        public ActionResult My(string id)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var trds = Db.Trades.ToList();
            if (id == "trades")
            {
                var trades = trds.Where(t => t.Seller.GroupId == currentUser.GroupId ||
                                                  t.Buyers.Select(c => c.Id)
                                                      .Intersect(currentUser.UserContragents.Select(v => v.Id))
                                                      .Any()).ToList()
                    .Select(c=> tradeDataLayer.CreateTradeViewModel(c, currentUser)).ToList();
                return View("Index", trades);
            }
            if (id == "orders")
            {
                var trades = trds.Where(t => !t.IsOffer&&t.IsOrder&&
                (t.Seller.GroupId == currentUser.GroupId || t.Buyers.Select(c => c.Id).Intersect(currentUser.UserContragents.Select(v => v.Id)).Any())).ToList()
                    .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser)).ToList();
                return View("../Orders/Index", trades);
            }
             if (id == "offers")
            {
                var trades = trds.Where(t => t.IsOffer && !t.IsOrder &&
                                                  (t.Seller.GroupId == currentUser.GroupId ||
                                                   t.Buyers.Select(c => c.Id)
                                                       .Intersect(currentUser.UserContragents.Select(v => v.Id))
                                                       .Any()))
                    .ToList()
                    .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser))
                    .ToList();
                return View("../Offers/Index", trades);
            }
            if (id == "docs")
            {
                return RedirectToAction("AllDocs");
            }
            return HttpNotFound();
        }



        [Authorize(Roles = "trader, byer, watcher")]
        [HttpGet, InLeftMenu]
        public ActionResult TradesHistory()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var trades =
                Db.TradesApproved()
                    .Where(f => !f.IsActual.Value && !f.IsFuture.Value && f.Seller.GroupId == currentUser.GroupId)
                    .ToList()
                    .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser))
                    .OrderByDescending(c => c.DateBegin)
                    .ToList();
            return View(trades);

        }


        [Authorize(Roles = "trader, root")]
        [HttpGet, InLeftMenu]
        public ActionResult TradesOnApproving()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var trades =
                Db.TradesNotApproved()
                .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser))
                .OrderByDescending(c => c.DateBegin)
                    .ToList();
            if (!User.IsInRole("root"))
            {
                trades = trades.Where(c => c.GroupId == currentUser.GroupId.ToString()).ToList();
            }
            return View(trades);
        }


        [Authorize(Roles = "trader, byer, watcher")]
        [HttpGet, InLeftMenu]
        public ActionResult BuyerTrades()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var trades =
                currentUser.UserContragents.SelectMany(
                    c => c.TradesListBuyer.Where(f => f.IsActual.Value || f.IsFuture.Value))
                    .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser))
                    .OrderByDescending(c => c.DateBegin)
                    .ToList();

            return View(trades);

        }
        [Authorize(Roles = "root")]
        public ActionResult DeleteTrade(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade table = Db.Trades.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);

        }
        [HttpPost, ActionName("Delete"), Authorize(Roles = "root")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trade trade = Db.Trades.Find(id);
            Db.Files.Remove(trade.ProductPassport);
            Db.Trades.Remove(trade);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "trader, byer")]
        [HttpGet, InLeftMenu]
        public ActionResult ShowCurrentTrades(string trade)
        {
            logger.Info("parsed tradeid:  " + trade);

            var tradeid = trade.Split('-')[1];
            logger.Info("parsed tradeid:  " + tradeid);
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currenttrade = Db.Trades.Find(int.Parse(tradeid));
            if (currenttrade.IsOffer)
            {
                return RedirectToAction("Index","Offers");
            }
            if (currenttrade.IsOrder)
            {
                return RedirectToAction("Index", "Orders");
            }
            if (currenttrade.Seller.ContragentUsers.Select(c => c.Id).ToList().Contains(currentUser.Id))
            {
                return RedirectToAction("SellerTrades");
            }
            if (currenttrade.Buyers.Any(f => f.ContragentUsers.Select(c => c.Id).ToList().Contains(currentUser.Id)))
            {
                return RedirectToAction("BuyerTrades");
            }
            if (currenttrade.IsFuture.Value)
            {
                return RedirectToAction("Index", "Trade", new { filter = "future" });
            }
            return RedirectToAction("Index", "Trade", new { filter = "actual" });

        }



        [Authorize(Roles = "trader, byer")]
        [HttpGet, InLeftMenu]
        public ActionResult SellerTrades()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var trades =
                currentUser.UserContragents.SelectMany(
                    c => c.TradesListSeller.Where(f => f.IsActual.Value || f.IsFuture.Value))
                    .Select(c => tradeDataLayer.CreateTradeViewModel(c, currentUser))
                    .OrderByDescending(c => c.DateBegin)
                    .ToList();
            return View(trades);
        }

        [Authorize(Roles = "trader, byer")]
        [HttpGet, InLeftMenu]
        public ActionResult SellerDocs()
        {

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
#warning Attention to files
            var docs =
                currentUser.UserContragents.SelectMany(f => f.OutBills)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new ShowTradeDocsModel(c)).ToList();

            return View("BuyerDocs", docs);

        }
        [Authorize(Roles = "trader, byer")]
        [HttpGet, InLeftMenu]
        public ActionResult AllDocs(string filter)
        {
          //  var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var contragents = User.Identity.GetUserContragents();
            var docs = Db.TradeBills.Where(c => contragents.Contains(c.ToContragentId)|| contragents.Contains(c.FromContragentId));
          //  var m = docs.ToList().Select(f => new ShowTradeDocsModel(f)).ToList();
            switch (filter)
            {
                case "offers":
                    docs = docs.Where(c => c.Trade.IsOffer);
                    break;
                case "orders":
                    docs = docs.Where(c => c.Trade.IsOrder);
                    break;              
                case "trades":
                    docs = docs.Where(c => !c.Trade.IsOrder&&!c.Trade.IsOffer);
                    break;
                default:
                    break;
            }
            return View("BuyerDocs", docs.ToList().Select(f=>new ShowTradeDocsModel(f)).ToList());

        }

        [Authorize(Roles = "trader, byer")]
        [HttpGet, InLeftMenu]
        public ActionResult BuyerDocs()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var docs =
                currentUser.UserContragents.SelectMany(f => f.InBills)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new ShowTradeDocsModel(c)).ToList();

            return View(docs);

        }



        [HttpGet]
        [Authorize]
        public async Task<ActionResult> RenderTradeView(int trade)
        {
            try
            {
                var auction = Db.Trades.Find(trade);
                var user =  await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (auction != null && user != null)
                {
                    if (auction.Type == TradeTypes.quotation.ToString())
                    {
                        foreach (var lot in auction.Lots)
                        {
                            await
                                tradeDataLayer.CheckTradeLot(lot);
                        }
                    }

                    var model = tradeDataLayer.CreateTradeViewModel(auction, user);

                    if (auction.IsOffer)
                        return PartialView("~/Views/Offers/_OfferTable.cshtml", model);
                    if (auction.IsOrder)
                        return PartialView("~/Views/Orders/_OrderTable.cshtml", model);
                    return PartialView("Trade/TableView", model);
                }
                return PartialView("_Error");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return PartialView("_Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Bet(BetViewModel model)
        {
            try
            {
                Db.Configuration.LazyLoadingEnabled = true;
                var currentuser = UserManager.FindById(User.Identity.GetUserId());

                var trade = Db.Trades.SingleOrDefault(c => c.Id == model.TradeId);
                if (trade == null)
                {
                    throw new ArgumentNullException("trade", "Trade must not br null");
                }
                var buyers = trade.Buyers.ToList();
                var users = buyers.SelectMany(c => c.ContragentUsers).Distinct().ToList();
                var buyer = Db.Contragents.SingleOrDefault(c => c.Id == model.BuyerId);
                var lot = Db.Lots.Find(model.LotId);
                if (lot == null)
                {
                    throw new ArgumentNullException("lot", "Lot must not br null");
                }
                if (users.All(c => c.Id != currentuser.Id) || !trade.IsActual.Value || buyer?.ContragentUsers.FirstOrDefault(c => c.Id == currentuser.Id) == null)
                {
                    logger.Error(
                        $"Кто-то слишком умный: {User.Identity.GetUserId()}, пытался ставку леваковую поставить");
                    throw new ArgumentException(LocalText.Inst.Get("error", "userBuyerError",
                        "Порушення безпеки", "Нарушение безопасности"));
                }
                //Продление торгов или времени лота
                await tradeDataLayer.CheckTradeTime(lot, users);
                //Проверка значений ставки
                await tradeDataLayer.CheckBetValues(trade, model, lot);
                //хардкод расчета объема
                model.Volume = model.LotsCount * trade.LotVolume;
                if (model.IsRebetting)
                {
                    var bettorebet = Db.Bets.Find(model.BetIdToRebet);
                    if (bettorebet == null)
                        throw new ArgumentNullException("bettorebet");
                    model.LotId = bettorebet.LotId;
                    await tradeDataLayer.ReBet(model, trade, bettorebet, currentuser);
                }
                else
                {
                    await tradeDataLayer.Bet(trade, model, lot, currentuser);
                }
               // trade.SendPopup(currentuser, hub, NotifyType.ToBuyerAboutHisBet);
                if (trade.Type != TradeTypes.quotation.ToString())
                {
                    await tradeDataLayer.CheckBets(trade);
                }
                foreach (var seller in trade.Seller.ContragentUsers.ToList())
                {
                    logger.Info($"{trade.Seller.ContragentUsers.ToList().Count}:update trade {trade.Id} table for user {seller.Email}");
                    await hub.UpdateTradeTable(trade.Id, seller.UserName);
                }
                foreach (var buyerUser in users.Distinct().ToList())
                {
                    await hub.UpdateTradeTable(trade.Id, buyerUser.UserName);
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
                    },
                        JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public async Task<ActionResult> BetTwo(int trade, int? lot = null)
        {
            try
            {
                var currentuser = UserManager.FindById(User.Identity.GetUserId());
                var tradeobject = Db.Trades.SingleOrDefault(c => c.Id == trade);
                if (tradeobject == null)
                {
                    throw new ArgumentNullException("trade");
                }
                var currentusercontragents = tradeobject.Buyers.Where(c => c.ContragentUsers.Any(f => f.Id == currentuser.Id)).ToList();
                var lotdb = tradeobject?.Lots.FirstOrDefault(c => c.Id == lot) ??
                            tradeobject.Lots.FirstOrDefault(c => !c.Bets.Any());
                if (tradeobject.Lots.All(f => f.Bets.Any(c => c.IsActual)))
                {
                    await hub.UpdateTradeTable(trade, currentuser.UserName);
                    throw new Exception("Empty free slots, please, try again");
                }
                if (lotdb != null && !lotdb.IsActual)
                {
                    throw new ArgumentException("Lot are not actual or empty");
                }

                var model = new BetViewModel(tradeobject, currentusercontragents, lotdb);
                if (tradeobject.Type == TradeTypes.quotation.ToString())
                {
                    return PartialView("Bets/_DifferencialBetModal", model);
                }
                if (tradeobject.MaxPrice.HasValue)
                {
                    return PartialView("Bets/_BetTwo", model);
                }
                return PartialView("Bets/_Bet", model);
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", GetFullErrorMessage(ex));
                return PartialView("Error", ModelState);
            }
        }

        [HttpGet]
        public ActionResult ReBet(int trade, int betid)
        {
            try
            {
                var currentuser = UserManager.FindById(User.Identity.GetUserId());
                //var tradeobject = db.Trades.FirstOrDefault(c=>c.Id==trade);
                var tradeobject = Db.Trades.Find(trade);
                if (tradeobject == null)
                {
                    throw new ArgumentNullException("trade");
                }
                var currentusercontragents = tradeobject.Buyers.Where(c => c.ContragentUsers.Any(f => f.Id == currentuser.Id));
                var bet = tradeobject.Bets.SingleOrDefault(c => c.Id == betid);
                if (bet != null && bet.IsActual)
                {
                    var model = new BetViewModel(tradeobject, currentusercontragents, bet.Lot, bet.Id)
                    {
                        //Тут я цену увеличиваю сразу
                        Price = bet.Price + tradeobject.PriceStep,
                        IsRebetting = true
                    };

                    if (tradeobject.Type == TradeTypes.quotation.ToString())
                    {
                        return PartialView("Bets/_DifferencialBetModal", model);
                    }
                    if (!tradeobject.MaxPrice.HasValue)
                    {
                        return PartialView("Bets/_Rebet", model);
                    }
                    return PartialView("Bets/_RebetTwo", model);
                }
                return PartialView("Error");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", GetFullErrorMessage(ex));
                return PartialView("Error", ModelState);
            }
        }

       
        [HttpGet]
        public ActionResult AcceptBetAndSaleLot(int lot)
        {
            try
            {
                var lotDb = Db.Lots.Find(lot);
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotDb);
                var model = new ResellBetViewModel()
                {
                    Price = lotDb.Price,
                    LotId = lotDb.Id,
                    Volume = lotDb.Volume
                };
                return PartialView("Modals/_AcceptBet", model);
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
                // return PartialView("Error", ModelState);
            };
        }

        [HttpPost]
        public async Task<ActionResult> AcceptBetAndSaleLot(ResellBetViewModel model)
        {
            try
            {
                var lotdb = Db.Lots.Find(model.LotId);
                if (lotdb == null)
                {
                    logger.Info("Какой-то чмошник что-то чудит");
                    throw new ArgumentNullException("lot", "Bet must not be empty");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotdb);
                await tradeDataLayer.OnRedemption(lotdb.Trade, lotdb.Bets.FirstOrDefault(c => c.IsActual), user);
                foreach (var u in lotdb.Trade.Buyers.SelectMany(c => c.ContragentUsers).ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
                }
                foreach (var u in lotdb.Trade.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
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

        [HttpGet]
        public ActionResult ResellBet(int lot)
        {
            try
            {
                var lotDb = Db.Lots.Find(lot);
                if (lotDb == null)
                {
                    throw new ArgumentNullException("lotDb", "Lot must not be null");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotDb);
                var model = new ResellBetViewModel()
                {
                    LotId = lotDb.Id,
                    Price = lotDb.Price,
                    Volume = lotDb.Volume
                };
                return PartialView("Modals/_ResellBet", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", GetFullErrorMessage(ex));
                return PartialView("Error", ModelState);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResellBet(ResellBetViewModel model)
        {
            try
            {
                var lotdb = Db.Lots.Find(model.LotId);
                if (lotdb == null)
                {
                    logger.Info("Какой-то чмошник что-то чудит");
                    throw new ArgumentNullException("bet", "Bet must not be empty");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotdb);
                if (model.Price < lotdb.MinPrice && lotdb.Bets.Any(c => c.IsActual))
                {
                    throw new ArgumentException(LocalText.Inst.Get("error", "Trade.ResellBet.MinPriceErrorSeller", "Ціна не може бути нижчою за ціну попередньої ставки", "Цена не может быть ниже предыдущей ставки"));
                }
                tradeDataLayer.ResellLot(lotdb, model.Price);
                foreach (var u in lotdb.Trade.Buyers.SelectMany(c => c.ContragentUsers).ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
                }
                foreach (var u in lotdb.Trade.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
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


        [HttpGet]
        public ActionResult DeclineBet(int lot)
        {
            try
            {
                var lotDb = Db.Lots.Find(lot);
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotDb);
                var model = new ResellBetViewModel()
                {
                    LotId = lotDb.Id,
                    Volume = lotDb.Volume,
                    Price = lotDb.Price
                };
                return PartialView("Modals/_DeclineBet", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", GetFullErrorMessage(ex));
                return PartialView("Error", ModelState);
            };
        }

        [HttpPost]
        public async Task<ActionResult> DeclineBet(ResellBetViewModel model)
        {
            try
            {
                var lotdb = Db.Lots.Find(model.LotId);
                if (lotdb == null)
                {
                    logger.Info("Какой-то чмошник что-то чудит");
                    throw new ArgumentNullException("lot", "Bet must not be empty");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                tradeDataLayer.CheckLotOnResell(user, lotdb);
                lotdb.IsActual = false;
                lotdb.IsSelled = false;
                lotdb.OnThinking = false;
                Db.UpdateEntity(lotdb, user.Id);
#warning Must send message to uses about declined lot

                foreach (var u in lotdb.Trade.Buyers.SelectMany(c => c.ContragentUsers).ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
                }
                foreach (var u in lotdb.Trade.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(lotdb.TradeId, u.UserName);
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
        [HttpGet]
        public ActionResult TakePart(int trade)
        {

            try
            {
                var currentUser = UserManager.FindById(User.Identity.GetUserId());
                var tradedb = Db.Trades.Find(trade);
                if (currentUser.GroupId == tradedb.Seller.GroupId)
                {
                    ModelState.AddModelError("other", "user from seller can not be buyer");
                    throw new AccessViolationException("user from seller can not be buyer");
                }
                var model = new TakePartModel()
                {
                    Contragent =
                       currentUser.Group.Contragents
                           .ToList()
                           .Select(v =>
                               new SelectListItem()
                               {
                                   Value = v.Id.ToString(),
                                   Text = v.LongName
                               }).ToList(),
                    TradeId = tradedb.Id
                };
                return PartialView("Modals/TakePartModal", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return PartialView("Error", ModelState);
            }


        }

        [HttpPost]
        public async Task<ActionResult> TakePart(string SelectedContragentsValue, int TradeId)
        {
            try
            {
                logger.Info($"Try parse contragents {SelectedContragentsValue} and trade: {TradeId}");


                var currentuser = UserManager.FindById(User.Identity.GetUserId());
                var auction = Db.Trades.Find(TradeId);
                foreach (var id in SelectedContragentsValue.Split(','))
                {
                    var buyer = Db.Contragents.Find(int.Parse(id));
                    auction.Buyers.Add(buyer);
                    Db.SaveChanges();
                   // auction.SendPopup(currentuser, hub, NotifyType.ToBuyerAboutSuccessIncludingToTrade);
                    await hub.UpdateTradeTable(auction.Id, currentuser.UserName);
                    foreach (var seller in auction.Seller.ContragentUsers.ToList())
                    {
                      //  auction.SendPopup(seller, hub, NotifyType.ToSellerAboutNewBuyerInTrade);
                        await hub.UpdateTradeTable(auction.Id, seller.UserName);

                    }
                    //     auction.SendPopups(auction.Seller.ContragentUsers.Distinct().ToList(), hub, NotifyType.ToSellerAboutNewBuyerInTrade);
                    logger.Info($"user {User.Identity.GetUserNamen(Db)} join to trade {auction.Id} with {buyer.LongName}");
                }

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new { Success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public ActionResult Leave(int trade)
        {

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var tradedb = Db.Trades.Find(trade);

            var model = new TakePartModel()
            {
                Contragent =
                         tradedb.Buyers.Where(c => c.ContragentUsers.Any(f => f.Id == currentUser.Id)).ToList()
                            .Select(v =>
                                new SelectListItem()
                                {
                                    Value = v.Id.ToString(),
                                    Text = v.LongName
                                }).ToList(),
                TradeId = tradedb.Id
            };
            return PartialView("Modals/_LeaveTrade", model);


        }

        [HttpPost]
        public async Task<ActionResult> Leave(string SelectedContragentsValue, int TradeId)
        {
            try
            {
                logger.Info($"Try parse contragents {SelectedContragentsValue} and trade: {TradeId}");

                var auction = Db.Trades.Find(TradeId);
                var currentuser = UserManager.FindById(User.Identity.GetUserId());
                foreach (var id in SelectedContragentsValue.Split(','))
                {
                    var buyer = auction.Buyers.FirstOrDefault(c => c.Id == int.Parse(id));
                    auction.Buyers.Remove(buyer);
                    Db.SaveChanges();
                  //  auction.SendPopup(currentuser, hub, NotifyType.AboutLeaveToLeaver);
                    await hub.UpdateTradeTable(auction.Id, currentuser.UserName);
                    foreach (var seller in auction.Seller.Group.Users.ToList())
                    {
                       // auction.SendPopup(seller, hub, NotifyType.AboutLeaveToSeller);
                        await hub.UpdateTradeTable(auction.Id, seller.UserName);
                    }
                    //  auction.SendPopups(auction.Seller.ContragentUsers.ToList(), hub, NotifyType.AboutLeaveToSeller);

                    logger.Info($"user {User.Identity.GetUserNamen(Db)} leave trade {auction.Id} with {buyer.LongName}");
                }

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new { Success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        [System.Web.Mvc.Authorize]
        public ActionResult CreateTrade()
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
             
                // var user = db.Users.SingleOrDefault(c=>c.Id==User.Identity.GetUserId());
                if (user == null)
                    throw new ArgumentNullException("user");
                if (user.UserContragents.Where(c => c.IsApproved).All(f => !f.IsSeller) || user.UserContragents.Count < 1)
                {
                    ModelState.AddModelError("other", LocalText.Inst.Get("error", "contragentBuyerError", "Юридична особа має бути активована як продавець", "Юридическое лицо должно быть активировано как продавец"));
                    return View("Error", ModelState);
                }
                var model = new NewAuctionViewModel(user, StaticData.Inst.GetCatalogModels().Where(c=>c.IsUsable));

                return View(model);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", ex.Message);
                return View("Error", ModelState);
            }
        }


        [HttpPost]
        public ActionResult CreateTrade(NewAuctionViewModel model)
        {
            try
            {
                if (model.SelectedTradeType.Code == TradeTypes.quotation.ToString())
                {
                    //хардкод врмени окончания торгов. Сорок два
                    model.TradeEnd = DateTime.UtcNow.AddYears(42);
                }

                if (model.TradeBegin > model.TradeEnd)
                {
                    logger.Error("wrong tradestart");
                    return
                        Json(
                            new
                            {
                                Success = false,
                                responseText =
                                    LocalText.Inst.Get("error", "dateswrong",
                                        "Час завершення торгів повинен бути більшим за час початку",
                                        "Время окончания торгов должно быть больше времени начала")
                            },
                            JsonRequestBehavior.AllowGet);
                }
#warning Хардокод на счет цены выкупа
                model.HasRedemptionPrice = true;

                tradeDataLayer.CreateTrade(model, User.Identity.GetUserId());
              
                EmailFactory.SendEmail("a.kuryanov@upklpg.com", "Новый торг на модерации",
                    "Уважаемый модератор, просьба одобрить новый торг:" + Url.Action("TradesOnApproving", "Trade", null, Request.Url.Scheme));
                EmailFactory.SendEmail("arystambekova.a@gmail.com", "Новый торг на модерации",
                    "Уважаемый модератор, просьба одобрить новый торг:" + Url.Action("TradesOnApproving", "Trade", null, Request.Url.Scheme));
                //Mailer.SendMail("a.kuryanov@upklpg.com", "Новый торг на модерации",
                //    "Уважаемый модератор, просьба одобрить новый торг:" + Url.Action("TradesOnApproving", "Trade", null, Request.Url.Scheme));



                return Json(new { Success = true, redirectUrl = Url.Action("Success", "Home") },
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




        public ActionResult EditTrade(int id)
        {
            var trade = Db.Trades.Find(id);
            if (trade == null || trade.IsFinallyApproved.Value)
            {
                throw new HttpException(404, "OOPS, something wrong");
            }
            if (User.IsInRole("root") || User.Identity.GetUserContragents().Contains(trade.SellerId))
            {
                var model = tradeDataLayer.CreateKnockoutEditTradeModel(trade, StaticData.Inst.GetCatalogModels());
                return View(model);
            }
            throw new HttpException(401, "Кто-то лезет не туда!");
        }
        [HttpPost]
        public ActionResult EditTrade(NewAuctionViewModel model)
        {
            try
            {
                model.IsPreApproved = false;
                model.IsAccepted = false;

                //   var dl = new TradeDataLayer(Db);
                //  dl.EditTrade(model, User.Identity.GetUserId());
                throw new NotImplementedException();
                // Mailer.SendMail("a.kuryanov@upklpg.com", "Торг почти одобрен",
                //"Уважаемый модератор,торг одобрен, ждем согласия юзверя: https://ptp.ua/approving");
                //return Json(new { Success = true, redirectUrl = Url.Action("Success", "Home") },
                //    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new { Success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AcceptTrade(string approveTradeId)
        {
            try
            {
                logger.Info($"try accept trade {approveTradeId} from user {User.Identity.GetUserId()}");
                var trade = Db.Trades.Find(Int32.Parse(approveTradeId));
                if (trade == null)
                {
                    return HttpNotFound();
                }

                if (User.IsInRole("trader") && trade.Seller.GroupId.ToString().Equals(User.Identity.GetUserGroup()))
                {
                    trade.IsAccepted = true;
                    trade.AcceptedByUserId = User.Identity.GetUserId();
                    trade.AcceptedDate = DateTime.Now;
                    Db.UpdateEntity(trade);
                    if (trade.IsFinallyApproved.Value)
                    {
                        var user = trade.Seller.ContragentUsers.FirstOrDefault();
                        EmailFactory.SendEmailAsync(new ApproveTrade(user, trade.Seller.LongName));
                    }
                }
                if (User.IsInRole("root"))
                {
                    trade.IsPreapproved = true;
                    trade.ApprovedByUserId = User.Identity.GetUserId();
                    trade.ApprovedDate = DateTime.Now;
                    await Db.UpdateEntityAsync(trade);

                    foreach(var user in trade.Seller.ContragentUsers.ToList())
                    {
                         EmailFactory.SendEmailAsync( new PreapproveTrade(user, trade.Seller.LongName));                        
                    }                   
                }
                if (trade.IsFinallyApproved.Value)
                {
                    // var ctr = db.Contragents.Where(c => c.IsApproved && c.IsBuyer).ToList();
                    foreach (var user in Db.Users.ToList())
                    {
                        EmailFactory.SendEmailAsync(new NewTrade(user, tradeDataLayer.CreateTradeViewModel(trade, user)));                       
                    }

                    await EmailFactory.Brodcast(new Broadcast() { Subject = "Новий торг", Body = "Запрошуємо до участі!" });
                }
                return RedirectToAction("TradesOnApproving");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", ex.Message);
                return View("Error", ModelState);
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Doc(int id, string param)
        {
            try
            {
                var model = tradeDataLayer.CreateDocModel(id, User.Identity.GetUserId(),Db);

                if (model != null)
                {
                    switch (param)
                    {
                        case "bill":
                            logger.Info("returning bill.pdf");
                            return !model.Buyer.IsResident || !model.Seller.IsResident ? new Rotativa.PartialViewAsPdf("Pdf/_ProductBillNerez", model) : new Rotativa.PartialViewAsPdf("Pdf/_ProductBill", model);
                        case "contract":
                            logger.Info("returning contract.pdf");
                            if (!model.Buyer.IsResident || !model.Seller.IsResident)
                            {
                                return model.Trade.ProductName != "polietylen"
                                    ? new Rotativa.PartialViewAsPdf("Pdf/_ContractNerez", model)
                                    {
                                        FileName = "contract.pdf",
                                        CustomSwitches = "--print-media-type --footer-center  \"____________________________                            ______________________________ \""
                                    }
                                    : new Rotativa.PartialViewAsPdf("Pdf/_PolietilenContract", model)
                                    {
                                        FileName = "contract.pdf",
                                        CustomSwitches = "--print-media-type --footer-center  \"____________________________                            ______________________________ \""
                                    };
                            }
                            else
                            {
                                return model.Trade.ProductName == "polietylen" ?
                                    new Rotativa.PartialViewAsPdf("Pdf/_PolietilenContract", model)
                                        {
                                            FileName = "contract.pdf",
                                            CustomSwitches = "--print-media-type --footer-center  \"____________________________                            ______________________________ \""
                                        }
                                    : new Rotativa.PartialViewAsPdf("Pdf/_ProductContract", model)
                                    {
                                        FileName = "contract.pdf",
                                        CustomSwitches = "--print-media-type --footer-center  \"____________________________                            ______________________________ \""
                                    };
                            }
                           
                        case "servicecontract":
                            logger.Info("returning contract.pdf");
                            return new Rotativa.PartialViewAsPdf("Pdf/_ServiceContract", model);
                        case "addcontract":
                            logger.Info("returning addcontract.pdf");

                            if (!model.Buyer.IsResident || !model.Seller.IsResident)
                            {
                                return model.Trade.ProductName != "polietylen" ?
                                    new Rotativa.PartialViewAsPdf("Pdf/_AddContractNerez", model) : new Rotativa.PartialViewAsPdf("Pdf/_AddPolietilenContract", model);

                            }
                            else
                            {
                                return model.Trade.ProductName == "polietylen" ?
                                    new Rotativa.PartialViewAsPdf("Pdf/_AddPolietilenContract", model) : new Rotativa.PartialViewAsPdf("Pdf/_AddContract", model);
                            }

                        //return !model.Buyer.IsResident || !model.Seller.IsResident ? new Rotativa.PartialViewAsPdf("Pdf/_AddContractNerez", model) : new Rotativa.PartialViewAsPdf("Pdf/_AddContract", model);

                        case "polietilencontract":
                            return !model.Buyer.IsResident || !model.Seller.IsResident ? new Rotativa.PartialViewAsPdf("Pdf/_PolietilenContract", model) : new Rotativa.PartialViewAsPdf("Pdf/_PolietilenContract", model);
                        default:
                            return View("Error");
                    }
                }
                return View("Error");
                //return RedirectToAction("Index", "Trade");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", LocalText.Inst.Get("error", "tradeOrBuyerNotfound", "Невірний набір параметрів для формування документу", "Неверный набор параметров для формирования документа"));
                return View("Error");
            }
        }

        public ActionResult AddContractNerez()
        {
            return new Rotativa.PartialViewAsPdf("Pdf/_AddContractNerez") { CustomSwitches = "--print-media-type --footer-center  \"The Seller____________________________  The Buyer______________________________ \"" };
        }

        public ActionResult ProductBillNerez()
        {
            return new Rotativa.PartialViewAsPdf("Pdf/_ProductBillNerez");
        }
        public ActionResult ContractNerez()
        {
            return new Rotativa.PartialViewAsPdf("Pdf/_ContractNerez") { CustomSwitches = "--print-media-type --footer-center  \"The Seller____________________________  The Buyer______________________________ \"" }; ;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Bill(int t, int b)
        {

            try
            {
                var tradebill = Db.TradeBills.SingleOrDefault(c => c.TradeId == t && c.ToContragentId == b);
                if (tradebill != null)
                {
                    return RedirectToAction("Doc", new { id = tradebill.Id, param = "bill" });
                }
                throw new ArgumentNullException("trade or bill not founded");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ModelState.AddModelError("other", LocalText.Inst.Get("error", "tradeOrBuyerNotfound", "Невірний набір параметрів для формування документу", "Неверный набор параметров для формирования документа"));
                return View("Error");
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_rolesManager != null)
                {
                    _rolesManager.Dispose();
                    _rolesManager = null;
                }


            }

            base.Dispose(disposing);
        }
    }
}
