using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NotificationsModule.Telegram;
using TradingPlatform.Data;
using TradingPlatform.Models;

namespace TradingPlatform.Controllers.api
{
    public class GcmController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string Get(string id)
        {
            var conf = db.Configs.FirstOrDefault(c => c.Key == "Telegram");
            if (conf != null)
            {
                conf.Value = id;
                db.UpdateEntity(conf);
                TeleAuth.Code = id;
            }
            else
            {
                conf = new Config() { Key = "Telegram", Value = id };
                TeleAuth.Code = id;
                db.Insert(conf);
            }
            return id;
        }
        public async Task Post(PushModel model)
        {
            var sen = new NotificationsModule.Push.GCM.GcmPushSender();

            var registration = db.Pushes.FirstOrDefault(c => c.DeviceId == model.DeviceId);
            if (String.IsNullOrWhiteSpace(registration?.UserId) && registration != null)
            {
                registration.UserId = model.User;
                await db.UpdateEntityAsync(registration,model.User);
            }

            if (db.Pushes.Any(c => c.DeviceId == model.DeviceId)&& !String.IsNullOrWhiteSpace(registration?.UserId))
                return;

            if (registration == null)
            {
                var result = await sen.Single(model.DeviceId, "Успішна реєстрація", "Вітаємо, Ви успішно підписані на оновлення від PTP!", "RegisterToPush");
                if (result)
                {
                    var newClient = new Push(model.DeviceId, model.User);
                    db.Insert(newClient,model.User);
                }
            
            }



        }
    }
}