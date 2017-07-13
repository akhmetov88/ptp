using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCM.Net;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Messaging
{
    public static class Push
    {
        public static async Task<bool> Single(string id, string title, string body, string campaign = null)
        {
            var serverKey = "";
            if (String.IsNullOrEmpty(campaign))
            {
                campaign = "trades";
            }

            var sender = new Sender(serverKey);
            var message = new Message
            {
                To = id,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    Icon = "https://ptp.ua/content/images/envelop.png",
                    ClickAction = $"https://ptp.ua/?utm_source=webpush&utm_campaign={campaign}&utm_medium=broadcast",
                }
            };
            var result = await sender.SendAsync(message);
            return result.MessageResponse.Success == 1;
        }
        /// <summary>
        /// Принимает идентификаторы для отправки
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="campaign"></param>
        /// <returns>Возвращает неудачные айдишники</returns>
        public static async Task<List<string>> Broadcast(Broadcast model, List<string> ids=null, string campaign = null)
        {
            if (ids==null)
            {
                using (var db = new ApplicationDbContext())
                {
#if DEBUG
                    ids = db.Pushes.Where(c=>c.User!=null&&c.User.IsDebug).Select(c => c.DeviceId).ToList();
#else
                    ids = db.Pushes.Select(c => c.DeviceId).ToList();
#endif
                }
            }
            var serverKey = "";
            if (String.IsNullOrEmpty(campaign))
            {
                campaign = "trade";
            }
            List<string> nonCorrect = new List<string>();
            var splittedIds = CustomSplit(ids);
            foreach (var id in splittedIds)
            {
                var sender = new Sender(serverKey);
                var message = new Message
                {
                    RegistrationIds = id,
                    Notification = new Notification
                    {
                        Title = model.Subject,
                        Body = model.Body,
                        Icon = "https://ptp.ua/content/images/envelop.png",
                        ClickAction = $"https://ptp.ua/?utm_source=webpush&utm_campaign={campaign}&utm_medium=broadcast",
                    }
                };
                var result = await sender.SendAsync(message);
                for (int i = 0; i < result.MessageResponse.Results.Count; i++)
                {
                    if (result.MessageResponse.Results[i].Error != null)
                    {
                        nonCorrect.Add(ids[i]);
                    }
                }
            }
            return nonCorrect;

        }

        private static List<List<T>> CustomSplit<T>(List<T> source)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 1000)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }

}
