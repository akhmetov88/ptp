using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TradingPlatform.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NLog;
using TradingPlatform.Enums;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Models
{
    public class NotificationHub : Hub
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
       
     

        public void GoToTrade(string user, int tradeId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(user).goToTrade("tradeTable-" + tradeId);
        }

        public void Reload(string user)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(user).reload();
        }

        /// <summary>
        /// Обновляет партиал вью, к котором отрисованы ставки и информация о торгах
        /// </summary>
        /// <param name="trade">айди торгов и айди блока, который надо обновить</param>
        /// <param name="userId">userName юзера, которому надо пушить обновление</param>
        public async Task UpdateTradeTable(int trade, string userName)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
           await context.Clients.User(userName).reloadTable(trade);
            // return Task.FromResult(0);
        }

        public void SendPopupps(Trade trade, NotifyType notifyType, IEnumerable<ApplicationUser> users)
        {
            try
            {
                switch (notifyType)
                {
                    case NotifyType.ToSellerAboutNewBuyerInTrade:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.ToSellerAboutNewBuyerInTrade, trade.Id)).ToList());
                        break;
                    case NotifyType.ToAllBuyersAboutStartTrade:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.ToAllBuyersAboutStartTrade, trade.Id)).ToList());
                        break;
                    case NotifyType.ToBuyerAboutHisBet:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.ToBuyerAboutHisBet, trade.Id)).ToList());
                        break;
                    case NotifyType.ToBuyerAboutSuccessIncludingToTrade:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.ToBuyerAboutSuccessIncludingToTrade, trade.Id)).ToList());
                        break;
                    case NotifyType.AboutRebet:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.AboutRebet, trade.Id)).ToList());
                        break;
                    case NotifyType.AboutTradeEnd:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.AboutTradeEnd, trade.Id)).ToList());
                        break;
                    case NotifyType.AboutLeaveToSeller:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.AboutLeaveToSeller, trade.Id)).ToList());
                        break;
                    case NotifyType.AboutLeaveToLeaver:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.AboutLeaveToLeaver, trade.Id)).ToList());
                        break;
                    case NotifyType.ToBuyersAboutContonuedTrade:
                        SendMessages(users.Distinct().Select(c => CreateMessage(c, NotifyType.ToBuyersAboutContonuedTrade, trade.Id)).ToList());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(notifyType), notifyType, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public void SendPopupp(Trade trade, NotifyType notifyType, ApplicationUser user, string fromUser = null, string buyer = null)
        {
            try
            {
                switch (notifyType)
                {
                    case NotifyType.ToSellerAboutNewBuyerInTrade:
                        SendMessage(CreateMessage(user, NotifyType.ToSellerAboutNewBuyerInTrade, trade.Id));
                        break;
                    case NotifyType.ToAllBuyersAboutStartTrade:
                        SendMessage(CreateMessage(user, NotifyType.ToAllBuyersAboutStartTrade, trade.Id));
                        break;
                    case NotifyType.ToBuyerAboutHisBet:
                        SendMessage(CreateMessage(user, NotifyType.ToBuyerAboutHisBet, trade.Id));
                        break;
                    case NotifyType.ToBuyerAboutSuccessIncludingToTrade:
                        SendMessage(CreateMessage(user, NotifyType.ToBuyerAboutSuccessIncludingToTrade, trade.Id));
                        break;
                    case NotifyType.AboutRebet:
                        SendMessage(CreateMessage(user, NotifyType.AboutRebet, trade.Id));
                        break;
                    case NotifyType.AboutTradeEnd:
                        SendMessage(CreateMessage(user, NotifyType.AboutTradeEnd, trade.Id));
                        break;
                    case NotifyType.AboutLeaveToLeaver:
                        SendMessage(CreateMessage(user, NotifyType.AboutLeaveToLeaver, trade.Id));
                        break;
                    case NotifyType.AboutLeaveToSeller:
                        SendMessage(CreateMessage(user, NotifyType.AboutLeaveToSeller, trade.Id));
                        break;
                    case NotifyType.ToBuyersAboutContonuedTrade:
                        SendMessage(CreateMessage(user, NotifyType.ToBuyersAboutContonuedTrade, trade.Id));
                        break;
                    case NotifyType.AboutSpecialRebet:
                        SendMessage(CreateMessage(user, NotifyType.AboutSpecialRebet, trade.Id, fromUser, buyer));
                        break;
                    case NotifyType.AboutWrongBet:
                        SendMessage(CreateMessage(user, NotifyType.AboutWrongBet, trade.Id, fromUser, buyer));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(notifyType), notifyType, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public NotificationViewModel CreateMessage(ApplicationUser user, NotifyType notifyType, int tradeNumber, string fromUser = null, string buyer = null)
        {
            string body = "";
            string subject = "";
            switch (notifyType)
            {
                case NotifyType.ToAllBuyersAboutStartTrade:
                    body = LocalText.Inst.Get("notifyMessage", "dearUser", "Шановний {userName}, торги № {tradeNumber} розпочато ", "Уважаемый {userName}, торги № {tradeNumber} начались");
                    subject = LocalText.Inst.Get("notifyMessage", "startTrade", "Торги розпочато", "Торги начались");
                    break;
                case NotifyType.ToBuyerAboutHisBet:
                    body = LocalText.Inst.Get("notifyMessage", "dearUserBetSuccess", "Шановний {userName}, Ваша ставка прийнята для участі в торгах", "Уважаемый {userName}, Ваша ставка принята для участия в торгах");
                    subject = LocalText.Inst.Get("notifyMessage", "dearUserBetSuccessSubj", "Ваша ставка прийнята", "Ваша ставка принята");
                    break;

                case NotifyType.ToSellerAboutNewBuyerInTrade:
                    body = LocalText.Inst.Get("notifyMessage", "tradeNewBuyer", "Шановний {userName}, у Ваших торгах з’явився новий учасник ", "Уважаемый {userName}, в Ваших торгах появился новый учасник");
                    subject = LocalText.Inst.Get("notifyMessage", "tradeNewBuyerTitle", "Новий учасник", "Новый участник");
                    break;
                case NotifyType.AboutTradeEnd:
                    subject = "Торги завершено";//LocalText.Inst.Get("notifyMessage", "tradeEndSubj", "Торги завершено", "Торги завершены");
                    body = "Торги {tradeNumber} завершено"; //LocalText.Inst.Get("notifyMessage", "tradeEndMessage", "{userName}, торги {tradeNumber} <a href='https://ptp.ua/trade'>почались</a>", "{userName}, торги {tradeNumber} <a href='https://ptp.ua/trade'>почались</a>");
                    break;
                case NotifyType.ToBuyerAboutSuccessIncludingToTrade:
                    body = LocalText.Inst.Get("notifyMessage", "tradeNewBuyerToBuyerMessage", "Шановний {userName}, ви успішно зареєстровані <p>для участі в торгах {tradeNumber}</p> ", "Уважаемый {userName}, Вы успешно зарегистрированы <p>для участия в торгах {tradeNumber}</p>");
                    subject = LocalText.Inst.Get("notifyMessage", "tradeNewBuyerToBuyerTitle", "Реєстрація", "Регистрация");
                    break;
                case NotifyType.AboutRebet:
                    body = LocalText.Inst.Get("notifyMessage", "tradeReBetToBuyer", "Шановний {userName}, Вашу ставку в торгах {tradeNumber} перебито ", "Уважаемый {userName}, Ваша ставка в торгах {tradeNumber} перебита");
                    subject = LocalText.Inst.Get("notifyMessage", "betNotActual", "Ставку перебито", "Ставка перебита");
                    break;
                case NotifyType.AboutLeaveToLeaver:
                    body = LocalText.Inst.Get("notifyMessage", "tradeLeaveToBuyer", "Шановний {userName},Вами покинуто торги № {tradeNumber} ", "Уважаемый {userName}, Вы покинули торг №  {tradeNumber}");
                    subject = LocalText.Inst.Get("notifyMessage", "imALeaver", "Торги покинуто", "Торги покинуты");
                    break;
                case NotifyType.AboutLeaveToSeller:
                    body = LocalText.Inst.Get("notifyMessage", "tradeLeaveToSeller", "Шановний {userName}, торги № {tradeNumber} покинуто одним учасником", "Уважаемый {userName}, торг {tradeNumber} покинут одним учасником");
                    subject = LocalText.Inst.Get("notifyMessage", "youHaveLeaver", "Вибуто з торгу", "Торг покинут");
                    break;
                case NotifyType.ToBuyersAboutContonuedTrade:
                    body = LocalText.Inst.Get("notifyMessage", "tradeContinuedToBuyer", "Шановний {userName}, торги № {tradeNumber} продовжено", "Уважаемый {userName}, торг № {tradeNumber} продлен");
                    subject = LocalText.Inst.Get("notifyMessage", "youHaveLeaver", "Продовження", "Продолжение");
                    break;
                case NotifyType.AboutSpecialRebet:
                    body = LocalText.Inst.Get("notifyMessage", "tradeBetSpecialRebettedCompany", "Шановний {userName}, Вашу ставку в торгах № {tradeNumber} перебив {buyerName}", "Уважаемый {userName}, Вашу ставку в торгах № {tradeNumber} перебил {buyerName}");
                    subject = LocalText.Inst.Get("notifyMessage", "specRebet", "Вас навмисне перебили", "Вас специально перебили");
                    break;
                case NotifyType.AboutWrongBet:
                    body = LocalText.Inst.Get("notifyMessage", "betError", "Шановний {userName}, Вашу ставку в торгах № {tradeNumber} не прийнято, спробуйте пізніше", "Уважаемый {userName}, Вашу ставку в торгах № {tradeNumber} не принято, попробуйте позже");
                    subject = LocalText.Inst.Get("notifyMessage", "wrongBet", "Ставку не прийнято", "Ставка не принята");
                    break;
                default:
                    logger.Error("Can not create popup");
                    break;
            }
            return new NotificationViewModel()
            {
                Body = body.Replace("{userName}", user.RegisterName).Replace("{tradeNumber}", tradeNumber.ToString()).Replace("{fromUser}", fromUser).Replace("{buyerName}", buyer),
                Subject = subject,
                CreateDate = DateTime.UtcNow,
                ToUserName = user.UserName,
                ToUserId = user.UserName,
            };
       }


        private static void SendMessages(List<NotificationViewModel> messages)
        {
            foreach (var message in messages.Distinct())
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.User(message.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(message)));
            }
        }

        private static void SendMessage(NotificationViewModel message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(message.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(message)));

        }

        public void SendPartial(string view)
        {
            //  string v = RenderPartialView("Trade", view, model);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.renderPartial(view);
        }

        public void SendMessageBetToUser(string user, string message)
        {
            //  string v = RenderPartialView("Trade", view, model);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(user).messageBet(message);
        }

        public void SetViewedNotification(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var notification = db.Notifications.SingleOrDefault(x => x.Id == id);
                if (notification != null)
                {
                    notification.IsViewed = true;
                    notification.ViewedDate = DateTime.UtcNow;
                    db.SaveChanges();
                }
            }
        }

        public static void SendNotification(int id)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var notification = db.Notifications.Select(x => new NotificationViewModel()
                    {
                        Id = x.Id,
                        IsViewed = x.IsViewed,
                        Subject = x.Subject,
                        Body = x.Body,
                        FromUserId = x.FromUserId,
                        FromUserName = x.FromUser.UserName,
                        ToUserId = x.ToUserId,
                        ToUserName = x.ToUser.UserName,
                        CreateDate = x.CreateDate,
                        ViewedDate = x.ViewedDate
                    }).SingleOrDefault(x => x.Id == id);

                    if (notification != null)
                    {
                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.User(notification.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(notification)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public static void SendNotification(Notification x)
        {
            try
            {
                var notification = new NotificationViewModel()
                {
                    //Id = x.Id,
                    IsViewed = x.IsViewed,
                    Subject = x.Subject,
                    Body = x.Body,
                    FromUserId = x.FromUserId,
                    FromUserName = x.FromUser.UserName,
                    ToUserId = x.ToUserId,
                    ToUserName = x.ToUser.UserName,
                    CreateDate = x.CreateDate,
                    ViewedDate = x.ViewedDate
                };

                if (notification != null)
                {
                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.User(notification.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(notification)));
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


    }
}
