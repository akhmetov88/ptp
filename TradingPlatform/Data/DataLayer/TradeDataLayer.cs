using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Enums;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.TradeModel;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Extentions;
using TradingPlatform.Messaging;

namespace TradingPlatform.Data.DataLayer
{
    public class TradeDataLayer
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private NotificationHub hub = new NotificationHub();
        public ApplicationDbContext db { get; private set; }

        public TradeDataLayer(ApplicationDbContext ctx)
        {
            db = ctx;
        }

        public async Task Bet(Trade trade, BetViewModel model, Lot lot, ApplicationUser currentuser)
        {

            var bet = new Bet(model, currentuser.Id);
            db.Insert(bet, currentuser.Id);

            lot.Price = model.Price.Value;
            lot.MinPrice = model.Price.Value;
            lot.BuyerId = bet.BuyerId;
            db.UpdateEntity(lot, currentuser.Id);

            SkipNotActualBets(lot);
            if (bet.IsRedemption)
            {
                await OnRedemption(trade, bet, currentuser);
            }
        }

        public async Task ReBet(BetViewModel model, Trade trade, Bet bettorebet, ApplicationUser currentuser)
        {
            try
            {
                await CheckBetValues(trade, model, bettorebet.Lot, currentuser);
                var bet = new Bet(model, currentuser.Id);
                db.Insert(bet, currentuser.Id);

                bettorebet.IsActual = false;
                bettorebet.IsRebetted = true;
                bettorebet.RebetterId = currentuser.Id;
                bettorebet.DateUpdate = DateTime.UtcNow;
                db.UpdateEntity(bettorebet, currentuser.Id);

                bettorebet.Lot.Price = bet.Price.Value;
                bettorebet.Lot.MinPrice = bet.Price.Value;
                bettorebet.Lot.BuyerId = bet.BuyerId;
                db.UpdateEntity(bettorebet.Lot, currentuser.Id);
                SkipNotActualBets(bet.Lot);
                foreach (var user in bettorebet.Buyer.ContragentUsers.ToList())
                {
                    if (trade.Type == TradeTypes.openFixed.ToString())
                    {
                         EmailFactory.SendEmailAsync(new RebetMail(user, bettorebet, bet));                      
                    }
                    if (trade.Type == TradeTypes.closeFixed.ToString())
                    {
                         EmailFactory.SendEmailAsync( new BetMail(user, bet));                       
                    }
                }
                if (bet.IsRedemption)
                {
                    await OnRedemption(trade, bet, currentuser);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        
        public void CheckLotOnResell(ApplicationUser user, Lot lot)
        {
            if (lot == null)
            {
                throw new ArgumentNullException("Lot is empty");
            }
            SkipNotActualBets(lot);
            if (lot.Trade.Seller.GroupId != user.GroupId)
            {
                throw new AccessViolationException("Security error");
            }
            if (!lot.IsActual || lot.IsSelled || !lot.OnThinking)
            {
                throw new ArgumentException("Lot is not actual");
            }
            if (lot.Bets.Any())
            {
                if (lot.Bets.All(c => !c.IsActual))
                {
                    if (lot.Price < lot.Bets.OrderBy(c => c.Id).LastOrDefault().Price)
                    {
                        throw new ArgumentException("Lot price cannot be less then last actual bet");
                    }
                }
                else
                {
                    if (lot.Price < lot.Bets.LastOrDefault(c => c.IsActual).Price)
                    {
                        throw new ArgumentException("Lot price cannot be less then last actual bet");
                    }
                }
            }
        }
        /// <summary>
        /// Проверка, чтоб не вышло, что актуальных заявок больше, чем всего топлива в предложении
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userOrderModel"></param>
        /// <param name="offer"></param>
        public void CheckOfferOnOrder(ApplicationUser user, UserOrderViewModel userOrderModel, Trade offer)
        {
            if(user.GroupId==offer.Seller.GroupId)
                throw new AccessViolationException("Security error");
            if (offer.TotalVolume - (offer.Orders.Where(c => c.IsAcceptedBySeller).Select(c => c.Volume).DefaultIfEmpty(0).Sum() + userOrderModel.Volume) < 0)
                throw new ArgumentOutOfRangeException("Volume error");
        }



        public async Task CheckTradeTime(Lot lot, List<ApplicationUser> users)
        {
            if ((lot.Trade.DateEnd - DateTime.UtcNow).TotalMinutes < 10 && lot.Trade.Type != TradeTypes.quotation.ToString())
            {
                //lot.Trade.DateEnd = lot.Trade.DateEnd.AddMinutes(10);
                lot.Trade.DateEnd = DateTime.UtcNow.AddMinutes(10);

                db.SaveChanges();
               // lot.Trade.SendPopups(users.Distinct().ToList(), hub, NotifyType.ToBuyersAboutContonuedTrade);
                foreach (var user in users)
                {
                     await hub.UpdateTradeTable(lot.Trade.Id, user.UserName);
                     EmailFactory.SendEmailAsync(new ExtendTrade(user, lot.Trade.Id, 10)); 
                }
                foreach (var mail in lot.Trade.Seller.ContragentUsers.ToList())
                {
                    await hub.UpdateTradeTable(lot.Trade.Id, mail.UserName);
                     EmailFactory.SendEmailAsync(new ExtendTrade(mail, lot.Trade.Id, 10));
                    //{
                    //    TradeNumber = lot.Trade.Id,
                    //    Min = "10",

                    //};
                    //m.Subject = "[Торги № " + lot.Trade.Id + "] " +
                    //            LocalText.Inst.Get("emailSubject", m.ToString(), "Продовження торгів",
                    //                "Продление торгов", mail.Locale);
                    //await m.SendMessageAsync();

                }
            }
            if (lot.Trade.Type == TradeTypes.quotation.ToString())
            {
                if ((lot.ElapsingTime - DateTime.UtcNow).TotalMinutes < 10)
                {
                    lot.ElapsingTime = DateTime.UtcNow.AddMinutes(10);
                    db.UpdateEntity(lot);
                }
            }
        }

        public void SkipNotActualBets(Lot lot)
        {
            if (lot.Trade.Lots.All(c => !c.IsActual || c.IsSelled)/*||trade.LotsCountAvailable.Value == 0*/)
            {
                //logger.Info($"Its {trade.LotsCountAvailable.Value}, closing trade ");
                lot.Trade.IsOpened = false;
                lot.Trade.DateEnd = DateTime.UtcNow;
                db.UpdateEntity(lot.Trade);
            }
            if (lot.Bets.Count() <= 1) return;
            foreach (var bet in lot.Bets.OrderByDescending(c => c.DateBet).Skip(1))
            {
                bet.IsActual = false;
                db.UpdateEntity(bet);
            }
        }




        public async Task CheckTradeLot(Lot lot, int minutes = 15, int resellingCount = 3)
        {
            if (lot == null)
            {
                throw new ArgumentNullException("lot", "Lot is empty");
            }
            SkipNotActualBets(lot);
            if (lot.Trade.Type != TradeTypes.quotation.ToString())
                return;

            if (lot.IsActual && !lot.IsSelled)
            {
                if (lot.OnThinking && lot.ElapsingTime <= DateTime.UtcNow)
                {
                    lot.OnThinking = false;
                    await OnElapsedTime(lot);
                    db.UpdateEntity(lot);
                }
                if (!lot.OnThinking && lot.ElapsingTime <= DateTime.UtcNow)
                {
                    if (lot.ReSellingCount >= resellingCount)
                    {
                        await OnElapsedTime(lot);
                        return;
                    }
                    lot.OnThinking = true;
                    lot.ElapsingTime = DateTime.UtcNow.AddMinutes(minutes);
                    await db.UpdateEntityAsync(lot);
                }
            }
        }

        public void ResellLot(Lot lot, decimal minPrice, int minutes = 15)
        {
            foreach (var bet in lot.Bets.Where(c => c.IsActual))
            {
                bet.IsActual = false;
                bet.IsRebetted = true;
                //bet.RebetterId = user.Id;
                db.UpdateEntity(bet);
            }
            lot.Price = minPrice;
            lot.MinPrice = minPrice;
            lot.OnThinking = false;
            lot.IsSelled = false;
            lot.IsActual = true;
            lot.BuyerId = (int?)null;
            lot.ReSellingCount++;
            lot.ElapsingTime = DateTime.UtcNow.AddMinutes(minutes);
            db.UpdateEntity(lot);
        }
        /// <summary>
        /// Метод, вызываемый для автоматических действий над лотом
        /// </summary>
        /// <param name="lot"></param>
        public async Task OnElapsedTime(Lot lot)
        {
            if (lot.Bets.Any(c => c.IsActual))
            {
                await OnRedemption(lot.Trade, lot.Bets.FirstOrDefault(c => c.IsActual), lot.Bets.FirstOrDefault(c => c.IsActual).AspNetUser);
            }
            else
            {
                lot.IsActual = false;
                lot.IsSelled = false;
                lot.OnThinking = false;
                await db.UpdateEntityAsync(lot);
            }
        }

        /// <summary>
        /// Все, что должно происходить при выкупе лота
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="bet"></param>
        /// <param name="currentuser"></param>
        public async Task OnRedemption(Trade trade, Bet bet, ApplicationUser currentuser)
        {
            try
            {
                bet.Lot.IsSelled = true;
                bet.Lot.IsActual = false;
                bet.Lot.OnThinking = false;
                db.UpdateEntity(bet.Lot, currentuser.Id);
                CreateBill(trade, bet.Buyer, currentuser);
                
                var z = new WonFix(trade, bet,bet.Buyer,currentuser);

                foreach (var userseller in trade.Seller.ContragentUsers.Distinct().ToList())
                {
                     EmailFactory.SendEmailAsync(new Redemption(userseller, bet));
         
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Все, что должно происходить при акцепте заявки по офферу
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="bet"></param>
        /// <param name="currentuser"></param>
        public async Task OnRedemption(Trade trade, Order order, ApplicationUser currentuser)
        {
            try
            {
                order.IsAcceptedBySeller = true;
                await db.UpdateEntityAsync(order, currentuser.Id);

                CreateBill(trade, order.Buyer, currentuser);

                foreach (var userseller in order.Buyer.Group.Users.ToList())
                {
                    EmailFactory.SendEmailAsync(new WonFix(trade, order, order.CreatedByUser));

                    //EmailFactory.SendEmailAsync(new OrderAccept(userseller, order));
                }
                //foreach (var userseller in trade.Seller.ContragentUsers.Distinct().ToList())
                //{
                //    EmailFactory.SendEmailAsync(new OrderAccept(userseller, order));                                    
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }


        private void CreateBill(Trade trade, Contragent buyer, ApplicationUser user)
        {
            try
            {

                if (!buyer.Bets.Where(c => c.TradeId == trade.Id).Any(c => c.IsActual || c.IsRedemption) && !trade.IsOffer && !trade.IsOrder)
                {
                    throw new ArgumentException("buyer dont have winned bets");
                }
                var contract = buyer?.BuyerContracts.FirstOrDefault(c => c.FromContragentId == trade.SellerId);
                if (contract == null)
                {
                    logger.Info($"Договор для покупателя {buyer?.LongName} от продавца {trade.Seller.LongName}");
                    contract = new Contract(trade, buyer, $"{trade?.DateBegin.ToString("yyMM-dd")}0{db.Contracts.Count() + 1}/PTP-T");
                    db.Insert(contract, user.Id);
                }
                logger.Info($"счет для покупателя {buyer?.LongName} от продавца {trade.Seller.LongName}");
                if (buyer.InBills.FirstOrDefault(c => c.TradeId == trade.Id) == null)
                {
                    var bill = new TradeBill(trade, buyer, contract, $"{db.TradeBills.Count(c => c.ContracttId == contract.Id) + 1}",
                        $"{db.TradeBills.Count(c => c.ContracttId == contract.Id) + 1}");
                    db.Insert(bill, user.Id);
                }
                else
                {
                    logger.Info($"Рахунок { db.TradeBills.Count(c => c.ContracttId == contract.Id) + 1}already created");
                }
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public string CreateDifferncialPrice(Trade trade)
        {
            return $"{StaticData.Inst.GetCatalogDesc(trade.DifferencialPriceValueType)} {LocalText.Inst.Get("text", "TradeDataLayer.CreateDifferncialPrice.Quotation", "котирування", "котировки")} " +
                   $"{StaticData.Inst.GetCatalogValue(trade.DifferencialPriceText)} {StaticData.Inst.GetCatalogDesc(trade.DifferencialPriceDateType)}";
        }

        /// <summary>
        /// Проверка значений пришедшей ставки. 
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="model"></param>
        /// <param name="lot"></param>
        /// <param name="currentuser"></param>
        public async Task CheckBetValues(Trade trade, BetViewModel model, Lot lot, ApplicationUser currentuser = null)
        {
            try
            {
                await CheckTradeLot(lot);

                if (lot.IsActual && !lot.IsSelled)
                {
                    if (model.LotsCount <= 0)
                    {
                        model.LotsCount = 1;
                    }
                    if (model.Price < lot.MinPrice && trade.Type == TradeTypes.quotation.ToString())
                    {
                        model.Price = lot.MinPrice;
                    }
                    if (trade.Type == TradeTypes.quotation.ToString() && lot.Bets.Any() && lot.Bets.All(c => c.Price >= model.Price))
                    {
                        model.Price = lot.Bets.Max(c => c.Price) + trade.PriceStep;
                    }
                    if (model.Price >= trade.DifferencialMax && trade.Type == TradeTypes.quotation.ToString())
                    {
                        model.Price = trade.DifferencialMax;
                        model.IsRedemption = true;
                    }
                    if (lot.Bets.Any() && model.Price < lot.Bets.LastOrDefault().Price)
                    {
                        throw new ArgumentException("Ціна ставки не може бути нище ціни останньої актуальної ставки");
                    }

                    if (lot.OnThinking)
                    {
                        throw new ArgumentException(LocalText.Inst.Get("error", "Trade.LotIsOnThinking", "Лот знаходиться на розгляді продавця", "Лот находится на рассмотрении продавцом"));
                    }
                    if (!trade.AllowBets.Value)
                    {
                        throw new ArgumentException(LocalText.Inst.Get("error", "maxLotsCount", "Перевищено допустиму кількість лотів", "Превышено допустимое количество лотов"));
                    }
                    if (model.Price >= trade.MaxPrice)
                    {
                        model.IsRedemption = true;
                        model.Price = trade.MaxPrice ?? model.Price;
                    }
                    if (!model.IsRedemption)
                    {
                        model.LotsCount = 1;
                    }
                }
                else
                {
                    throw new ArgumentException(LocalText.Inst.Get("error", "Trade.LotIsNotActualException", "Лот не активний або викуплено", "Лот неактивен или выкуплен"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //  trade.SendPopup(currentuser, hub, NotifyType.AboutWrongBet,"fromUser?","buyer/?");
                throw;
            }

        }
        public NewAuctionViewModel CreateKnockoutEditTradeModel(Trade trade, List<CatalogModel> catalogs)
        {
            try
            {
                //  var catalogs = db.Catalogs.Where(c => !c.ParentId.HasValue).ToList().Select(m => new CatalogModel(m)).ToList();
                var model = new NewAuctionViewModel()
                {
                    TaxForUpTime = trade.TaxForUpTime,
                    Contact = trade.Contact,
                    CurrencySelected = StaticData.Inst.GetCatalogModel(trade.Currency),
                    DayToPay = trade.DayToPay,
                    TransportTypeSelected = StaticData.Inst.GetCatalogModel(trade.TransportType),
                    TradeTypes = catalogs.Where(c => c.Code == "Типы торгов").ToList(),
                    CountryList = catalogs.Where(c => c.Code == "Страны").ToList(),
                    ProductsList = catalogs.Where(c => c.Code == "Справочник товаров").ToList(),
                    TransportTypes = catalogs.Where(c => c.Code == "Виды транспорта").ToList(),
                    RailwayTransportTherms = catalogs.Where(c => c.Code == "Варианты оборота цистерн").ToList(),
                    Currencies = catalogs.SingleOrDefault(c => c.Code == "Валюты").Dependencies.ToList(),

                    DaysForUptime = trade.DaysForUptime,
                    DeliveryDateFrom = trade.DeliveryDateFrom,
                    DeliveryDateTo = trade.DeliveryDateTo,
                    HasRedemptionPrice = trade.MaxPrice.HasValue,
                    IncothermSelected = StaticData.Inst.GetCatalogModel(trade.Incothermns),
                    LotVolume = trade.LotVolume ?? 0m,
                    LotsCount = trade.LotsCount ?? 0,
                    MinBetVolume = trade.MinVolumeBet ?? 0,
                    MinStepVolume = trade.MinVolumeStep ?? 0m,
                    PriceStep = trade.PriceStep ?? 0,
                    RailwayBeginSelected = StaticData.Inst.GetCatalogModel(trade.RailwayBegin),
                    RailwayEndSelected = StaticData.Inst.GetCatalogModel(trade.RailwayEnd),
                    RedemptionPrice = trade.MaxPrice,
                    SelectedBank = new BankBillViewModel(trade.BankBill),
                    ContragentList = trade.Seller.Group.Contragents.Select(c => new ContragentTradeViewModel(c)).ToList(), //new List<ContragentTradeViewModel>() { new ContragentTradeViewModel() { BankBills = new List<BankBillViewModel>() { new BankBillViewModel(trade.BankBill) }, Id = trade.SellerId, LongName = trade.Seller.LongName } },
                    //SelectedCountry = GetSelectedCatalogModel(trade.ProductCountry, db),
                    SelectedNomenclature = StaticData.Inst.GetCatalogModel(trade.ProductNomenclature),
                    SelectedPlant = StaticData.Inst.GetCatalogModel( trade.ProductManufacturer),
                    SelectedProduct = StaticData.Inst.GetCatalogModel(trade.ProductName),
                    SelectedQuality = StaticData.Inst.GetCatalogModel(trade.ProductQuality),
                    SelectedSeller = new ContragentTradeViewModel(trade.Seller), //{ BankBills = new List<BankBillViewModel>() { new BankBillViewModel(trade.BankBill) }, Id = trade.SellerId, LongName = trade.Seller.LongName },
                    SelectedTradeType = StaticData.Inst.GetCatalogModel(trade.Type),
                    StartPrice = trade.PriceStart,
                    TransportPointSelected = StaticData.Inst.GetCatalogModel(trade.ShipmentPoint),
                    TotalVolume = trade.TotalVolume ?? 0,
                    TradeBegin = trade.DateBegin,//.ToClientTimeDate(),
                    TradeEnd = trade.DateEnd,//.ToClientTimeDate(),
                    TradeId = trade.Id,
                    IsAccepted = trade.IsAccepted,
                    ProductPassportId = trade.ProductPassport?.Id,
                    IsPreApproved = trade.IsPreapproved,
                    fileData = new fileData()
                };
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

        }
        public Trade CreateTrade(NewAuctionViewModel model, string userId)
        {
            try
            {
                logger.Info($"try create trade");
                var trade = new Trade()
                {
                    BankBillId = model.SelectedBank.Id,
                    IsFixed = model.SelectedTradeType.Code == TradeTypes.closeFixed.ToString() || model.SelectedTradeType.Code == TradeTypes.openFixed.ToString(),
                    DeliveryDateFrom = model.DeliveryDateFrom,
                    DeliveryDateTo = model.DeliveryDateTo,
                    DateBegin = model.TradeBegin.ToUniversalTime().AddSeconds(-1 * model.TradeBegin.Second),
                    DayToPay = model.DayToPay.ToUniversalTime(),
                    LotVolume = model.LotVolume,
                    PriceStart = model.StartPrice,
                    MaxPrice =
                       model.HasRedemptionPrice && model.RedemptionPrice != 0
                           ? model.RedemptionPrice
                           : (decimal?)null,
                    LotsCount = model.LotsCount,
                    SellerId = model.SelectedSeller.Id,
                    PriceStep = model.PriceStep ?? 0.5m,
                    ProductCountry = model.SelectedCountry?.Code ?? String.Empty,
                    ProductQuality = model.SelectedQuality?.Code ?? String.Empty,
                    ProductManufacturer = model.SelectedPlant?.Code ?? String.Empty,
                    ProductName = model.SelectedProduct?.Code ?? String.Empty,
                    ProductNomenclature = model.SelectedNomenclature?.Code ?? String.Empty,
                    MinVolumeBet = model.MinBetVolume,
                    MinVolumeStep = model.MinStepVolume,
                    TotalVolume = model.TotalVolume == 0 ? model.LotsCount * model.LotVolume : model.TotalVolume,
                    ShipmentPoint = model.TransportPointSelected?.Code ?? String.Empty,
                    TransportType = model.TransportTypeSelected?.Code ?? String.Empty,
                    DateEnd = model.TradeEnd.ToUniversalTime().AddSeconds(-1 * model.TradeEnd.Second),
                    IsOpened = true,
                    //   LotsCountAvailable = model.LotsCount,
                    IsProcessed = false,
                    CreatedByUserId = userId,
                    Contact = model.Contact ?? String.Empty,
                    Incothermns = model.IncothermSelected?.Code ?? String.Empty,
                    Type = model.SelectedTradeType?.Code ?? String.Empty,
                    Currency = model.CurrencySelected?.Code ?? String.Empty,
                    Unit = model.SelectedUnit?.Code ?? String.Empty,
                    TaxForUpTime = model.TaxForUpTime ?? 0,
                    DaysForUptime = model.DaysForUptime ?? 0,
                    RailwayBegin = model.RailwayBeginSelected?.Code ?? String.Empty,
                    RailwayEnd = model.RailwayEndSelected?.Code ?? String.Empty,
                    IsPreapproved = false,
                    IsAccepted = false,
                    FirstTherms = CreateFirsTradeTherms(model),
                    PriceCurrency = model.SelectedPriceCurrency?.Code ?? String.Empty,
                    DifferencialPriceText = model.DifferencialPriceTextSelected?.Code ?? String.Empty,
                    DifferencialPriceDateType = model.DifferencialPriceDateTypeSelected?.Code ?? String.Empty,
                    DifferencialPriceValueType = model.DifferencialPriceValueTypeSelected?.Code ?? String.Empty,
                    DifferencialMin = model.DifferencialMin ?? 0,
                    DifferencialMax = model.DifferencialMax ?? 100000000,
                    TankTherms = CreateTankTherms(model.DaysForUptime ?? 0, model.RailwayBeginSelected?.Code, model.RailwayEndSelected?.Code, model.TaxForUpTime ?? 0, model.CurrencySelected?.Code),
                    DifferencialPriceValue = 0,
                    DifferencialPriceDateTypeDesc = model.DifferencialPriceDateTypeDescSelected?.Code ?? String.Empty,
                    IsOffer = model.IsOffer,
                    IsOrder = model.IsOrder,                    
                };
                db.Insert(trade, userId);
                if (!UploadFileFromJs(model.fileData, trade.Id)&&!trade.IsOrder)
                {
                    throw new ArgumentNullException("product passport for trade must not be empty");
                }
                if(!model.IsOffer&&!model.IsOrder)
                    CreateTradeLots(trade, userId);

                return trade;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        private void CreateTradeLots(Trade trade, string userId, int minuutesToElapse = 15)
        {
            for (int i = 1; i <= trade.LotsCount.Value; i++)
            {
                var lot = new Lot()
                {
                    TradeId = trade.Id,
                    IsActual = true,
                    Volume = trade.LotVolume.Value,
                    SellerId = trade.SellerId,
                    Price = trade.DifferencialMin,
                    MinPrice = trade.DifferencialMin,
                    IsSelled = false,
                    ElapsingTime = trade.DateBegin.AddMinutes(minuutesToElapse),
                    OnThinking = false,
                    ReSellingCount = 1,
                    LotNumber = i

                };
                db.Insert(lot, userId);
            }
        }

        private string CreateFirsTradeTherms(NewAuctionViewModel model)
        {
            return $"Старт: {model.StartPrice}";
        }

        //public void EditTrade(NewAuctionViewModel model, string userId)
        //{
        //    try
        //    {
        //        logger.Info($"try edit trade");
        //        var trade = db.Trades.Find(model.TradeId);
        //        if (!trade.IsFinallyApproved.Value && !trade.IsActual.Value)
        //        {
        //            trade.BankBillId = model.SelectedBank.Id;
        //            trade.IsFixed = model.SelectedTradeType.Code == TradeTypes.closeFixed.ToString() ||
        //                      model.SelectedTradeType.Code == TradeTypes.openFixed.ToString();
        //            trade.DeliveryDateFrom = model.DeliveryDateFrom;
        //            trade.DeliveryDateTo = model.DeliveryDateTo;
        //            trade.DateBegin = model.TradeBegin.ToUniversalTime().AddSeconds(-1 * model.TradeBegin.Second);
        //            trade.DayToPay = model.DayToPay.ToUniversalTime();
        //            trade.LotVolume = model.LotVolume;
        //            trade.PriceStart = model.StartPrice;
        //            trade.MaxPrice =
        //                model.HasRedemptionPrice && model.RedemptionPrice != 0
        //                    ? model.RedemptionPrice
        //                    : (decimal?)null;
        //            trade.LotsCount = model.LotsCount;
        //            trade.SellerId = model.SelectedSeller.Id;
        //            trade.PriceStep = model.PriceStep??0.5m;
        //            trade.ProductCountry = model.SelectedCountry.Code;
        //            trade.ProductQuality = model.SelectedQuality.Code;
        //            trade.ProductManufacturer = model.SelectedPlant.Code;
        //            trade.ProductName = model.SelectedProduct.Code;
        //            trade.ProductNomenclature = model.SelectedNomenclature.Code;
        //            trade.MinVolumeBet = model.MinBetVolume;
        //            trade.MinVolumeStep = model.MinStepVolume;
        //            trade.TotalVolume = model.TotalVolume == 0 ? model.LotsCount * model.LotVolume : model.TotalVolume;
        //            trade.ShipmentPoint = model.TransportPointSelected.Code;
        //            trade.TransportType = model.TransportTypeSelected.Code;
        //            trade.DateEnd = model.TradeEnd.ToUniversalTime().AddSeconds(-1 * model.TradeEnd.Second);
        //            trade.IsOpened = true;
        //            trade.LotsCountAvailable = model.LotsCount;
        //            trade.IsProcessed = false;
        //            trade.Contact = model.Contact;
        //            trade.Incothermns = model.IncothermSelected.Code;
        //            trade.Type = model.SelectedTradeType.Code;
        //            trade.Currency = model.CurrencySelected?.Code;
        //            trade.Unit = "т";
        //            trade.TaxForUpTime = model.TaxForUpTime ?? 0;
        //            trade.DaysForUptime = model.DaysForUptime ?? 0;
        //            trade.RailwayBegin = model.RailwayBeginSelected?.Code;
        //            trade.RailwayEnd = model.RailwayEndSelected?.Code;
        //            trade.IsAccepted = model.IsAccepted;
        //            trade.IsPreapproved = model.IsPreApproved;
        //            trade.ApprovedByUserId = trade.IsPreapproved ? userId : null;
        //            trade.AcceptedByUserId = trade.IsAccepted ? userId : null;

        //            trade.ApprovedDate = trade.IsPreapproved ? DateTime.UtcNow : (DateTime?)null;
        //            trade.AcceptedDate = trade.IsAccepted ? DateTime.UtcNow : (DateTime?)null;
        //            trade.EditedDate = DateTime.UtcNow;
        //            //var timeOffSet = HttpContext.Current.Session["timezoneoffset"];
        //            //var timediff = HttpContext.Current.Session["timediff"];
        //            db.UpdateEntity(trade, userId);

        //            if (model.fileData?.base64String != null)
        //            {
        //                if (!UploadFileFromJs(model.fileData, trade.Id))
        //                {
        //                    throw new ArgumentNullException("product passport for trade must not be empty");
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        throw;
        //    }
        //}

        public bool UploadFileFromJs(fileData file, int tradeId)
        {
            try
            {
                var trade = db.Trades.Find(tradeId);
                if (file == null || trade == null)
                {
                    return false;
                }

                var content = GetBytesFromBase64(file.base64String);
                logger.Info($"Try get content from passport {content.Length}");
                if (trade.ProductPassport == null)
                {
                    var doc = new File()
                    {
                        UserId = trade.CreatedByUserId,
                        Content = content,
                        ContragentId = trade.SellerId,
                        ContentType = file.file.type,
                        IsApproved = true,
                        UpdateDate = DateTime.UtcNow,
                        FileType = db.FileTypes.FirstOrDefault(c => c.Name == "QualityPassport"),
                        Comment = "",
                        CreateDate = DateTime.UtcNow,
                        FileName = String.IsNullOrEmpty(file.file.name)
                                ? $"ProductPassport {DateTime.Now.Ticks}"
                                : file.file.name,
                        ApprovedByUserId = trade.CreatedByUserId

                    };
                    db.Insert(doc);

                    trade.ProductPassport = doc;
                    db.UpdateEntity(trade);
                    return true;
                }
                trade.ProductPassport.Content = content;
                trade.ProductPassport.ContentType = file.file.type;
                trade.ProductPassport.FileName = file.file.name;
                db.UpdateEntity(trade);
                return true;
            }


            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }


        public byte[] GetBytesFromBase64(string b64input)
        {
            try
            {
                logger.Info("Try get bytes from base64 string");
                return !String.IsNullOrEmpty(b64input) ? Convert.FromBase64String(b64input) : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Проверяет уже сделанные ставки по торгам, ставит флаг неактуальности
        /// </summary>
        /// <param name="tradeid">идентификатор торгов</param>
        public async Task CheckBets(Trade trade)
        {
            try
            {
                // var trade = db.Trades.Find(tradeid);
                var actualbets = GetCurrentTradeActualBets(trade).ToList();
                var actualBetsCountWithLots = actualbets.Count(c => !c.IsRedemption) +
                                              actualbets.Where(c => c.IsRedemption).Sum(c => c.LotsCount);
                logger.Info($"trade found: {trade.Id}, betscount = {actualbets.Count}");
                if (trade.IsFixed.Value)
                {

                    if (actualBetsCountWithLots >= trade.LotsCount.Value)
                    {
                        trade.PriceStart = actualbets.Min(c => c.Price.Value) + trade.PriceStep.Value;
                        db.UpdateEntity(trade);
                        //logger.Info($"Price is up at trade {traded} price = {trade.PriceStart}");


                    }
                    if (actualbets.Sum(c => c.LotsCount) == trade.LotsCount.Value && (actualbets.All(c => c.Price == trade.MaxPrice) || actualbets.Where(c => c.IsRedemption).Sum(c => c.LotsCount) >= trade.LotsCount.Value))
                    {
                        trade.IsOpened = false;
                        db.UpdateEntity(trade);
                        //send popup about trade closing
                       // trade.SendPopups(trade.Buyers.SelectMany(c => c.ContragentUsers).Distinct(), hub, NotifyType.AboutTradeEnd);
                        logger.Info($"Price is MAX trade {trade.Id} is closed");
                    }

                    IEnumerable<Bet> betstoNotActual =
                        actualbets.Skip(actualbets.Count(c => c.IsRedemption)).Skip(trade.LotsCountAvailable.Value).ToList();
                    if (actualbets.Where(c => c.IsRedemption).Sum(c => c.LotsCount) >= trade.LotsCount.Value || trade.LotsCountAvailable.Value <= 0)
                    {
                        logger.Info("пытаюсь оставить только выкупные ставки");
                        //    betstoNotActual = actualbets.Except(actualbets.Where(c => c.IsRedemption).ToList());
                        List<Bet> redempBets = new List<Bet>();
                        foreach (var redemptionbet in actualbets.Where(c => c.IsRedemption))
                        {
                            if (redempBets.Sum(c => c.LotsCount) < trade.LotsCount.Value)
                                redempBets.Add(redemptionbet);
                            else if (redempBets.Sum(c => c.LotsCount) == trade.LotsCount.Value)
                            {
                                break;
                            }
                        }
                        betstoNotActual = actualbets.Except(redempBets).ToList();
                        trade.IsOpened = false;
                        db.UpdateEntity(trade);
                    }

                    foreach (var bet in betstoNotActual.Distinct())
                    {
                      //  trade.SendPopup(bet.AspNetUser, hub, NotifyType.AboutRebet);
                        bet.IsActual = false;
                        var m = new BetMail(bet.AspNetUser, bet);             

                        db.UpdateEntity(bet, bet.AspNetUserId);
                    }
                }
                else
                {

#warning Define logic for variable trades
                    logger.Info("insert logic");
                }

#pragma warning restore CS1030 // #warning: 'Define logic for variable trades'

            }
            catch (Exception ex)
            {
                logger.Error(ex);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeid"></param>
        /// <returns></returns>
        public List<Bet> GetCurrentTradeActualBets(int tradeid)
        {
            var trade = db.Trades.Find(tradeid);
            if (trade.LotsCount != null && trade.IsFixed.Value)
                return trade.Bets.Where(c => c.IsActual).OrderByDescending(c => c.IsRedemption).ThenByDescending(c => c.Price).ThenBy(f => f.DateBet).ToList();

            return trade.Bets.Where(c => c.IsActual).OrderByDescending(c => c.Price).ThenBy(f => f.DateBet).TakeWhile(x => x.IsActual).ToList();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeid"></param>
        /// <returns></returns>
        public List<Bet> GetCurrentTradeAllBets(int tradeid)
        {
            var trade = db.Trades.Find(tradeid);
            if (trade.LotsCount != null && trade.IsFixed.Value)
                return trade.Bets.OrderByDescending(c => c.IsRedemption).ThenByDescending(c => c.Price).ThenBy(f => f.DateBet).ToList();

            return trade.Bets.OrderByDescending(c => c.Price).ThenBy(f => f.DateBet).TakeWhile(x => x.IsActual).ToList();
        }

        public List<Bet> GetCurrentTradeActualBets(Trade trade)
        {

            if (trade.LotsCount != null && trade.IsFixed.Value)
                return trade.Bets.Where(c => c.IsActual).OrderByDescending(c => c.IsRedemption).ThenByDescending(c => c.Price).ThenBy(f => f.DateBet).ToList();

            return trade.Bets.Where(c => c.IsActual).OrderByDescending(c => c.Price).ThenBy(f => f.DateBet).TakeWhile(x => x.IsActual).ToList();

        }

        /// <summary>
        /// Возвращает список ставок для текущего юзера
        /// </summary>
        /// <param name="selectedBets">Список актуальных ставок, количество соответствует количеству лотов</param>
        /// <param name="contragents">айдишники контрагентов, имена которых надо отдать на вьюшку</param>
        /// <returns></returns>
        public List<BetViewModel> GetCurrentUserBetTable(Trade auction, List<int> contragents)
        {

            //var auction = db.Trades.Find(trade);
            var actualbets = GetCurrentTradeActualBets(auction);
            List<BetViewModel> currentbets = new List<BetViewModel>();
            try
            {
                if (contragents.Any())
                {
                    currentbets = actualbets.Select(c => new BetViewModel()
                    {
                        Id = c.Id,
                        DateBet = c.DateBet,
                        TradeId = c.TradeId,
                        Price = c.Price.Value,
                        LotsCount = c.LotsCount,
                        Volume = c.Volume.Value,
                        MaxPrice = auction.MaxPrice,
                        // Sum = c.Volume.Value*c.Price.Value,
                        IsCurrentUserBet = contragents.Contains(c.BuyerId),
                        BuyerId = c.BuyerId,
                        IsActual = c.IsActual,
                        IsRedemption = c.IsRedemption,
                        //  BuyerName = c.AspNetUser.RegisterName,
                        BuyerName = c.Buyer.LongName,
                        DateEnd = auction.DateEnd,
                        Step = auction.PriceStep,

                    }).ToList();
                }
                else
                {
                    currentbets = actualbets.Select(c => new BetViewModel()
                    {
                        Id = c.Id,
                        BuyerId = c.Id,
                        IsActual = c.IsActual,
                        DateBet = c.DateBet,
                        DateUpdate = c.DateUpdate,
                        TradeId = c.TradeId,
                        Price = c.Price.Value,
                        LotsCount = c.LotsCount,
                        Volume = c.Volume.Value,
                        BuyerName = c.Buyer.LongName,
                        IsCurrentUserBet = contragents.Contains(c.BuyerId),
                        IsRedemption = c.IsRedemption,
                        DateEnd = auction.DateEnd,
                        MaxPrice = auction.MaxPrice,

                    }).ToList();
                }
                return currentbets;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return currentbets;
            }
        }

        /// <summary>
        /// Возвращает массив идентификаторов контрагентов, которыми управляет текущий юзер. 
        /// Если юзер не аутентифицирован, возвращает пустой список
        /// </summary>
        /// <param name="userId">айди пользователя</param>
        /// <returns>list int ContragentId </returns>
        //public List<int> GetUsersContragentsId(string userId)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(userId)) return new List<int>();
        //        var user = db.Users.Find(userId);
        //        return user.UserContragents.Select(c => c.Id).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return new List<int>();
        //    }

        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trade">объект торгов из БД</param>
        /// <param name="userId"></param>
        /// <param name="filter"></param>
        /// <param name="isAuction"></param>
        /// <returns></returns>
        public TradeViewModel CreateTradeViewModel(Trade trade, ApplicationUser user, string filter = null)
        {
            try
            {
                if (!trade.IsOffer && !trade.IsOrder)
                {
                    foreach (var lot in trade.Lots)
                    {
                        CheckTradeLot(lot);
                    }
                }

                return new TradeViewModel()
                {
                    IsFixed = trade.IsFixed.Value,
                    TotalVolume = trade.TotalVolume,
                    BankBillId = trade.BankBillId,
                    SellerName = trade.Seller.LongName,
                    BankBill = new BankBillViewModel(trade.BankBill),
                    Id = trade.Id,
                    LotVolume = trade.LotVolume??0,
                    DateBegin = trade.DateBegin,
                    LotsCount = trade.LotsCount,
                    DateEnd = trade.DateEnd,
                    DeliveryDateTo = trade.DeliveryDateTo,
                    DeliveryDateFrom = trade.DeliveryDateFrom,
                    ShipmentPoint = trade.ShipmentPoint??String.Empty,
                    ProductName = trade.ProductName ?? String.Empty,
                    PriceStep = trade.PriceStep??0,
                    ProductNomenclature = trade.ProductNomenclature ?? String.Empty,
                    TransportType = trade.TransportType ?? String.Empty,
                    ProductPassportId = trade.ProductPassport?.Id,
                    MaxPrice = trade.MaxPrice??trade.PriceStart,
                    ProductQuality = trade.ProductQuality ?? String.Empty,
                    MinVolumeBet = trade.MinVolumeBet??0,
                    ProductCountry = trade.ProductCountry ?? String.Empty,
                    PriceStart = trade.PriceStart,
                    MinVolumeStep = trade.MinVolumeStep,
                    SellerId = trade.SellerId,
                    ProductManufacturer = trade.ProductManufacturer ?? String.Empty,
                    Unit = trade.Unit ?? String.Empty,
                    IsActual = trade.IsActual.Value,
                    IsFuture = trade.IsFuture.Value,
                    IsPast = trade.IsPast.Value,
                    Filter = filter ?? "actual",
                    AllContragentIds = trade.Buyers.Select(c => c.Id).ToList(),
                    CurrentUserContragentIds = user.UserContragents.Select(c => c.Id).ToList(),
                    ActualBets = GetCurrentUserBetTable(trade, user.UserContragents.Select(c => c.Id).ToList()),
                    IsOpened = trade.IsOpened,
                    LotsCountAvailable = trade.LotsCountAvailable,
                    GroupId = trade.Seller.GroupId.ToString(),
                    DayToPay = trade.DayToPay,
                    ContactUser = trade.Contact ?? String.Empty,
                    Incotherms = trade.Incothermns,
                    TankTherms = CreateTankTherms(trade.DaysForUptime, trade.RailwayBegin, trade.RailwayEnd, trade.TaxForUpTime, trade.Currency),
                    TradeType = trade.Type??"closeFixed",
                    IsApproved = trade.IsFinallyApproved.Value,
                    IsAccepted = trade.IsAccepted,
                    IsPreapproved = trade.IsPreapproved,
                    PriceCurrency = trade.PriceCurrency,
                    DifferencialMax = trade.DifferencialMax,
                    DifferencialMin = trade.DifferencialMin,
                    DifferencialPriceDateType = trade.DifferencialPriceDateType,
                    DifferencialPriceText = trade.DifferencialPriceText,
                    DifferencialPriceValue = trade.DifferencialPriceValue,
                    DifferencialPriceValueType = trade.DifferencialPriceValueType,
                    Lots = trade.Lots != null&&trade.Lots.Any()?trade.Lots.Select(f => new LotViewModel(f, user)).ToList():null,
                    DifferencialPriceDesc = CreateDifferncialPrice(trade),
                    DifferencialPriceDateTypeDesc = trade.DifferencialPriceDateTypeDesc,
                    IsOffer = trade.IsOffer,
                    IsOrder = trade.IsOrder,
                    Orders = trade.Orders!=null&&trade.Orders.Any() ? trade.Orders.Select(c => new Models.OfferModels.OrderViewModel(c, user)).ToList() : null,
                    VolumeAvailable = trade.GetFreeOfferVolume()
                };

            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
        }



        private string CreateTankTherms(int days, string begin, string end, int tax, string currency)
        {
            try
            {
                return $@"{LocalText.Inst.Get("text", "textBegin", "Період з дати прибуття завантажених цистерн", "Период с даты прибытия загруженных цистерн")}  
                        {StaticData.Inst.GetCatalogDesc(begin)}  {LocalText.Inst.Get("text", "toDateTherms", "до дати", "до даты")} 
                        {StaticData.Inst.GetCatalogDesc(end)} {LocalText.Inst.Get("text", "periodThermsNew", "не повинен перевищувати", "не должен превышать")}  {days} 
({NumberToWords.Number.ToWords((decimal)days, new NumberToWords.Unit(NumberToWords.GenitiveNumber.Neuter, "", "", ""))})
{LocalText.Inst.Get("text", "thermsCalendarDays", "календарних дні(в)", "календарных дня(ей)")}. {LocalText.Inst.Get("text", "periodTherms", "Період, який перевищує", "Период, который превышает")} 
{days} ({NumberToWords.Number.ToWords((decimal)days, new NumberToWords.Unit(NumberToWords.GenitiveNumber.Neuter, "", "", ""))})
{LocalText.Inst.Get("text", "thermsCalendarDays", "календарних дні(в)", "календарных дня(ей)")},
{LocalText.Inst.Get("text", "periodDaysTherms", "календарні дні, оплачується в розмірі", "календарных дня, оплачивается в розмере")}  {tax} {StaticData.Inst.GetCatalogValue(currency)} 
                        {LocalText.Inst.Get("text", "payDayTherms", "в добу за кожну цистерну.", "в сутки за каждую цистерну.")}";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return LocalText.Inst.Get("error", "emptyTherms",
                    "Під час генерації умов транспортування виникла помилка",
                    "Во время генерирования условий транспортировки возникла ошибка");
            }
        }


        /// <summary>
        /// Моделька документов 
        /// </summary>
        /// <param name="billid">bill id</param>
        /// <param name="userId">айди юзера, который смотрит вьюшку</param>
        /// <returns></returns>
        public BillModel CreateDocModel(int billid, string userId, ApplicationDbContext ctx=null)
        {
            try
            {
                ContragentDataLayer dl = new ContragentDataLayer(ctx??db);

                ShowTradeDocsModel bills;
                var user =  db.Users.Find(userId);
                var bill = db.TradeBills.Find(billid);

                var trade = bill.Trade;
                var buyer = bill.Buyer;
                var buyerbets = !trade.IsOffer&&!trade.IsOrder? bill.Buyer?.Bets?.Where(c => (c.IsActual || c.IsRedemption) && c.TradeId == bill.TradeId).ToList().Select(c => new BetViewModel(c, user)).ToList()
                    :
                    bill.Trade.Orders.Where(c=>c.BuyerId==buyer.Id).Select(f=>new BetViewModel(f,user)).ToList();
                //if (!bill.Trade.IsClosedByBills)
                //{
                //    buyerbets = buyerbets?.Where(c => c.IsRedemption).ToList();
                //    bills = new ShowTradeDocsModel(bill);
                //    if (buyer != null && bills != null && buyerbets.Any() &&
                //        (user.GroupId == buyer.GroupId || user.GroupId == bill.Trade.Seller.GroupId) || user.Email == "a.kuryanov@upklpg.com")
                //    {
                //        var model = new BillModel()
                //        {
                //            Trade = CreateTradeViewModel(trade, user),
                //            Seller = dl.FillContragent(trade.Seller),
                //            Buyer = dl.FillContragent(buyer),
                //            ShowModel = bills,
                //            Bets = buyerbets,
                //        };
                //        return model;
                //    }
                //}
                //else
                //{
                    //if()
                    bills = new ShowTradeDocsModel(bill);
                    if (trade != null && buyer != null && bills != null && buyerbets.Any() &&
                        (user.GroupId == buyer.GroupId || user.GroupId == trade.Seller.GroupId) || user.Email == "a.kuryanov@upklpg.com" || user.Email == "admin@ptp.ua")
                    {
                        var model = new BillModel()
                        {
                            Trade = CreateTradeViewModel(trade, user),
                            Seller = dl.FillContragent(trade.Seller),
                            Buyer = dl.FillContragent(buyer),
                            ShowModel = bills,
                            Bets = buyerbets//.Select(c => new BetViewModel(c, user)).ToList(),
                        };
                        return model;
                    }
                //}
                return null;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }



        public void CreateContracts(int tradeid)
        {

            try
            {
                var trade = db.Trades.Find(tradeid);
                if (trade == null) throw new ArgumentNullException(nameof(Trade));
                foreach (var buyer in trade.Buyers.Where(c => c.BuyerContracts.FirstOrDefault(f => f.FromContragentId == trade.SellerId) == null).ToList())
                {
                    if (db.Contracts.Any(c => c.FromContragentId == trade.SellerId && c.ToContragentId == buyer.Id))
                    {
                        logger.Info($"ALREADY have contract from {trade.Seller.LongName} to {buyer.LongName}");
                        continue;
                    }
                    if (!buyer.InBills.Any(c => c.FromContragentId == trade.SellerId && c.TradeId == tradeid))
                    {
                        continue;
                    }
                    logger.Info($"Creating contract from {trade.Seller.LongName} to {buyer.LongName}");
                    var contract = new Contract(trade, buyer, $"{trade.DateBegin.ToString("yyMM-dd")}0{db.Contracts.Count() + 1}/PTP-T");
                    db.Insert(contract);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }
    }
}