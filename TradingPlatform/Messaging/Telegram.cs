using System;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;
//using WebSessionStore = NotificationsModule.Telegram.WebSessionStore;

namespace TradingPlatform.Messaging
{

    public static class Telegram
    {
        private static TelegramClient _client;
        //  public static bool IsAuthenticated = false;
        public static string Code = String.Empty;
        public static string Hash = String.Empty;
        public static string Phone= String.Empty;

        public static TelegramClient client => _client ?? (_client = new TelegramClient(000000, "0000000", new WebSessionStore()));

              

        public static async Task AuthUser()
        {
            await client.ConnectAsync();            
            if (client.IsUserAuthorized())
            {
                return;
            }
            if (!client.IsUserAuthorized())
            {              
                Hash = await client.SendCodeRequestAsync(Phone);
                
                await Task.Delay(10000);
                //if (string.IsNullOrWhiteSpace(Code))
                //    await AuthUser();
                using (var db = new ApplicationDbContext())
                {
                    Code = db.Configs.SingleOrDefault(c => c.Key.ToLower() == "telegram")?.Value;
                }
                //Thread.Sleep(10000);
                //await AuthUser();
            }
            //await client.ConnectAsync();
            TLUser user = null;
            try
            {
                user = await client.MakeAuthAsync(Phone, Hash, Code);
                //  IsAuthenticated = client.IsUserAuthorized();
            }
            catch (CloudPasswordNeededException ex)
            {
                var password = await client.GetPasswordSetting();
                var password_str = "";
                user = await client.MakeAuthWithPasswordAsync(password, password_str);
            }
            catch (InvalidPhoneCodeException ex)
            {
              //  throw new Exception("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram", ex);
                EmailFactory.SendEmail("admin@ptp.ua", "Exception", "CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram"+ex.Message);
            }
        }
        /// <summary>
        /// Отправка сообщения в Телеграм-канал
        /// </summary>
        /// <param name="model"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task Broadcast(Broadcast model, string channel="ptpua",string campain=null)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(campain))
                    campain = "trades";
#if DEBUG
                channel = "ptpuatest";
#endif
                var message = $"{model.Subject}:\n{model.Body}\n\nДетальніше на PTP: \n https://goo.gl/qq4W2T";
                await client.ConnectAsync();
                if (client.IsUserAuthorized())
                {                    
                    var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
                    var chat = dialogs.chats.lists
                        .OfType<TLChannel>()
                        .FirstOrDefault(c => c.title.ToLower() == channel);
                    await client.SendMessageAsync(new TLInputPeerChannel() { channel_id = chat.id, access_hash = chat.access_hash.Value }, message);
                }
                else
                {
                    await AuthUser();
                }
            }
            catch (Exception ex)
            {
                EmailFactory.SendEmail("admin@ptp.ua", "TelegramException", ex.Message);
                if(ex.Message.Contains("SESSION_REVOKED"))
                {
                    var file = System.Web.HttpContext.Current.Server.MapPath("~/Content/docs/session.dat");
                    EmailFactory.SendEmail("admin@ptp.ua", "TelegramException", file +" exists: "+ System.IO.File.Exists(file));
                    
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                        return;// await Broadcast(model, "ptpuatest");
                    }
                }
            }
           
        }

        public static async Task<bool> CheckNumber(string number)
        {
            await client.ConnectAsync();
            return await client.IsPhoneRegisteredAsync(number);

        }
        public static async Task SendMessageTest(string message, string number = null)
        {

            try
            {
                // this is because the contacts in the address come without the "+" prefix
                var normalizedNumber = number ?? Phone;

                await client.ConnectAsync();

                var result = await client.GetContactsAsync();

                var user = result.users.lists
                    .OfType<TLUser>()
                    .FirstOrDefault(x => x.phone == normalizedNumber);

                if (user == null)
                {
                    throw new System.Exception("Number was not found in Contacts List of user: " + "");
                }

                //await client.SendTypingAsync(new TLInputPeerUser() { user_id = user.id });
                //Thread.Sleep(3000);
                await client.SendMessageAsync(new TLInputPeerUser() { user_id = user.id }, message);
            }
            catch (Exception)
            {

            }
        }
    }
}
