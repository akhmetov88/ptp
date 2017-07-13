using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace NotificationsModule.Telegram
{
    public class Telegram
    {
        private static Telegram _instance ;
        private bool _isSendedRequest = false;
        
        private  static WebSessionStore session
        {
            get
            {
                return session ?? new WebSessionStore();
            }
            set
            {
                if (session==null)
                {
                    session = value;
                }
            }
        }

        private Telegram()
        {
            
        }

        public static Telegram Instance()
        {
            
            return _instance ?? (_instance = new Telegram());
        }

        public static TelegramClient Client
        {
            get
            {
                return Client ?? new TelegramClient(0000, "", session);
            }
            set { Client = value; }
        }

        public  virtual async Task Auth()
        {
            await Client.ConnectAsync();
            string hash = String.Empty;
            string code = String.Empty;
            if (!_isSendedRequest)
            {
                hash = await Client.SendCodeRequestAsync("");
                _isSendedRequest = true;
            }
            code = ConfigurationManager.AppSettings["CodeToAuthenticate"];
            if (String.IsNullOrWhiteSpace(code))
            {
                Thread.Sleep(10000);
                await Auth();
            }

            TLUser user = null;
            try
            {
                user = await Client.MakeAuthAsync("", hash, code);
            }
            catch (CloudPasswordNeededException ex)
            {
                var password = await Client.GetPasswordSetting();
                var password_str = "";
                user = await Client.MakeAuthWithPasswordAsync(password, password_str);
            }
            catch (InvalidPhoneCodeException ex)
            {
                throw new Exception("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",
                                    ex);
            }
        }
        public virtual async Task SendMessageToChannelTest()
        {
            var dialogs = (TLDialogs)await Client.GetUserDialogsAsync();
            var chat = dialogs.chats.lists
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.title == "PTP");

            await Client.SendMessageAsync(new TLInputPeerChannel() { channel_id = chat.id, access_hash = chat.access_hash.Value }, "TEST MSG");
        }
    }


}
