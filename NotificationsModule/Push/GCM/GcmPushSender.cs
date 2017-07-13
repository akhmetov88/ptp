using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCM.Net;

namespace NotificationsModule.Push.GCM
{
    public class GcmPushSender
    {
        public async Task<bool> Single(string id, string title, string body, string campaign = null)
        {
            var serverKey = "AAAAG8K8rfY:APA91bFas80IlcnLoUpDAjPcxeCH8t7JyIUZz91t-BaFgl9kL7XqjtOQXu7plP4zLB_J8JtMaCAMWMKqrwP3v4mypgUNjbEiPj7aqEiC1kdoQvDFSSGafr91CgEuYCPyyuq4exx-t5XE";
            if (String.IsNullOrEmpty(campaign))
            {
                campaign = "trade";
            }

            var sender = new Sender(serverKey);
            var message = new Message
            {
                To = id,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    Icon = "https://ptp.ua/content/images/logo.png",
                    ClickAction = $"https://ptp.ua/?utm_source=webpush&utm_campaign={campaign}&utm_medium=broadcast",
                }
            };
            var result =  await sender.SendAsync(message);
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
        public async Task<List<string>> Broadcast(List<string> ids, string title, string body, string campaign = null)
        {
            var serverKey = "AAAAG8K8rfY:APA91bFas80IlcnLoUpDAjPcxeCH8t7JyIUZz91t-BaFgl9kL7XqjtOQXu7plP4zLB_J8JtMaCAMWMKqrwP3v4mypgUNjbEiPj7aqEiC1kdoQvDFSSGafr91CgEuYCPyyuq4exx-t5XE";
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
                        Title = title,
                        Body = body,
                        Icon = "https://ptp.ua/content/images/logo.png",
                        ClickAction = $"https://ptp.ua/?utm_source=webpush&utm_campaign={campaign}&utm_medium=broadcast",
                    }
                };
                var result = await sender.SendAsync(message);
                for (int i = 0; i < result.MessageResponse.Results.Count; i++)
                {
                    if (result.MessageResponse.Results[i].Error!=null)
                    {
                       nonCorrect.Add(ids[i]);
                    }
                }
            }
            return nonCorrect;

        }

        private List<List<T>> CustomSplit<T>(List<T> source)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 1000)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }

}
