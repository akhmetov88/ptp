using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
using TradingPlatform.Enums;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;
using TradingPlatform.Messaging;

namespace TradingPlatform.Data.DataLayer
{
    public class JobDataLayer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ApplicationDbContext _context;
        private NotificationHub _notificationHub;
        public JobDataLayer(ApplicationDbContext context)
        {
            _context = context;
            _notificationHub = new NotificationHub();
        }

        /// <summary>
        /// Отправить всплывающие уведомления и письма о начале торгов
        /// </summary>
        /// <returns></returns>
        public async Task SendNotificationsAboutNewTrade()
        {
            foreach (var trade in GetCreatedTrades())
            {
                // trade.SendPopups(trade.Buyers.SelectMany(c => c.ContragentUsers).Distinct().ToList(), _notificationHub, NotifyType.ToAllBuyersAboutStartTrade);
                foreach (var buyer in trade.Buyers.SelectMany(c => c.ContragentUsers).Distinct().ToList())
                {
                     //EmailFactory.SendEmailAsync(new NewTrade(buyer, trade.Id));
                    _notificationHub.GoToTrade(buyer.UserName, trade.Id);
                }
                foreach (var seller in trade.Seller.ContragentUsers.Distinct().ToList())
                {
                    // EmailFactory.SendEmailAsync(new NewTrade(seller, trade.Id));
                    _notificationHub.GoToTrade(seller.UserName, trade.Id);
                }
                trade.IsProcessed = true;
                await _context.UpdateEntityAsync(trade);
            }
        }
        /// <summary>
        /// Успешные торги
        /// </summary>
        /// <returns></returns>
        public List<Trade> GetSuccesTrades()
        {
            try
            {
                var trades = _context.Trades.ToList();
                var closedtrades = trades.Where(c => c.IsSuccefullyClosed.Value && !c.IsClosedByBills).ToList();
                if (closedtrades.Any())
                {
                    logger.Info($"try to finish {closedtrades.Count} trades");
                }
                return closedtrades.Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        /// <summary>
        /// Неуспешные торги
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Trade> GetFailedTrades()
        {
            try
            {
                var trades = _context.Trades.ToList().Where(c => !c.IsSuccefullyClosed.Value && !c.IsClosedByBills).ToList();
                return trades;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public void CloseTrade(Trade trade)
        {
            try
            {
                logger.Info($"try close trade {trade.Id}");
                // trade.SendPopups(trade.Buyers.SelectMany(c => c.ContragentUsers).Distinct().ToList(), _notificationHub, NotifyType.AboutTradeEnd);
                foreach (var user in trade.Seller.ContragentUsers.ToList().Distinct())
                {
                    logger.Info($"Send seller letter to {user.Email}");
                    int winnerscount = TradeWinners(trade).Count();
                     EmailFactory.SendEmailAsync(new SellerLetter(user, trade, winnerscount));
                }
                foreach (var buyer in TradeWinners(trade).Distinct())
                {
                    CreateBill(trade.Id, buyer.Id);

                    foreach (var user in GetContragentUsers(buyer).Distinct().ToList())
                    {
                        var bets =
                            buyer.Bets.Where(f => f.TradeId == trade.Id && (f.IsActual || f.IsRedemption))
                                .ToList();
                        #region SendMail to winner
                        logger.Info($"send WINNER leter to {user.Email}");
                         EmailFactory.SendEmailAsync(new WonFix(trade, bets, buyer, user));
                    }
                    #endregion

                    foreach (var loser in TradeLosers(trade.Id).ToList().Distinct().ToList())
                    {
                         EmailFactory.SendEmailAsync(new LoserLetter(loser, trade.Id, trade.Seller.LongName));

                    }

                    foreach (var failedtrade in GetFailedTrades())
                    {
                        logger.Info($"NEED TO SEND LETTERS ABOUT FAILED TRADE: {failedtrade.Id}");
                        failedtrade.IsClosedByBills = true;
                        _context.UpdateEntity(failedtrade);
                    }


                }
                logger.Info($"CLOSING trade {trade.Id}");
                trade.IsClosedByBills = true;
                _context.UpdateEntity(trade);
            }


            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }



        /// <summary>
        /// Получить список торгов, по которым не было отправлено уведомление об их начале
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Trade> GetCreatedTrades()
        {
            try
            {
                var trades = _context.Trades.ToList().Where(c => c.IsActual.Value && !c.IsProcessed && c.IsFinallyApproved.Value).ToList();
                return trades;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        /// <summary>
        /// Список покупателей-участников торга
        /// </summary>
        /// <param name="trade">объект торга</param>
        /// <returns></returns>
        private IEnumerable<Contragent> GetTradeContragents(Trade trade)
        {
            try
            {
                var contragents = trade.Buyers.Distinct().ToList();
                return contragents;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        /// <summary>
        /// Список юзеров-пользователей контрагентов
        /// </summary>
        /// <param name="contragent"></param>
        /// <returns></returns>
        private IEnumerable<ApplicationUser> GetContragentUsers(Contragent contragent)
        {
            try
            {
                List<ApplicationUser> userss = new List<ApplicationUser>();
                var users = contragent.ContragentUsers.Distinct().ToList();
                foreach (var user in users)
                {
                    if (!userss.Contains(user))
                    {
                        userss.Add(user);
                    }
                }
                return users;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }



        /// <summary>
        /// Прикручиваем счета на оплату
        /// </summary>
        /// <param name="tradeId">с какого торга </param>
        /// <param name="buyerId">кому</param>
        public void CreateBill(int tradeId, int buyerId)
        {
            try
            {
                var trade = _context.Trades.Find(tradeId);
                if (trade == null)
                {
                    throw new ArgumentNullException("trade not found");
                }
                var buyer = trade.Buyers.FirstOrDefault(c => c.Id == buyerId);
                if (buyer == null)
                {
                    throw new ArgumentNullException("buyer");
                }
                if (!buyer.Bets.Where(c => c.TradeId == tradeId).Any(c => c.IsActual || c.IsRedemption))
                {
                    throw new ArgumentException("buyer dont have winned bets");
                }
                var contract = buyer?.BuyerContracts.FirstOrDefault(c => c.FromContragentId == trade.SellerId);
                if (contract == null)
                {
                    logger.Info($"Договор для покупателя {buyer?.LongName} от продавца {trade.Seller.LongName}");
                    contract = new Contract(trade, buyer, $"{trade.DateBegin.ToString("yyMM-dd")}0{_context.Contracts.Count() + 1}/PTP-T");
                    _context.Insert(contract);
                }
                logger.Info($"счет для покупателя {buyer?.LongName} от продавца {trade.Seller.LongName}");
                if (buyer.InBills.FirstOrDefault(c => c.TradeId == tradeId) == null)
                {
                    var bill = new TradeBill(trade, buyer, contract, $"{_context.TradeBills.Count(c => c.ContracttId == contract.Id) + 1}", $"{_context.TradeBills.Count(c => c.ContracttId == contract.Id) + 1}");
                    _context.Insert(bill);
                }

            }

            catch (Exception ex)
            {
                logger.Error(ex);
                // throw;
            }
        }

        public IEnumerable<Contragent> TradeWinners(Trade trade)
        {
            try
            {
                var winners = trade.Bets.Where(c => c.IsActual || c.IsRedemption).ToList().Select(f => f.Buyer).Distinct().ToList();
                logger.Info($"Winners founded:{winners.Count}");
                return winners;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public IEnumerable<ApplicationUser> TradeLosers(int tradeId)
        {
            try
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var trade = _context.Trades.Find(tradeId);
                var manyusers = trade.Buyers.Where(f => f.Bets.Where(c => c.TradeId == trade.Id)
                    .All(b => !b.IsActual)).ToList().SelectMany(c => c.ContragentUsers).Distinct().ToList();
                //  var losers = trade?.Bets.Select(f => f.Buyer).ToList().Where(f => f.Bets.Where(c => c.TradeId == trade.Id)
                //    .All(b => !b.IsActual)).ToList().SelectMany(c => c.ContragentUsers).Distinct().ToList();
                logger.Info($"Losers founded: {manyusers.Count} ");
                foreach (var user in manyusers)
                {
                    if (!users.Contains(user))
                        users.Add(user);
                    else
                        logger.Info($"Loser {user.Email} already in collection");
                }
                return users;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

    }
}
