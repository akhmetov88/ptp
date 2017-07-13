using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace NotificationsModule.Telegram
{

    public static  class TeleAuth
    {
        private static TelegramClient _client;
      //  public static bool IsAuthenticated = false;
        public static string Code = String.Empty;
        public static string Hash = String.Empty;


        public static TelegramClient client => _client ?? (_client = new TelegramClient(00000, "", new WebSessionStore()));

        public static async Task Init()
        {
            await client.ConnectAsync();
            if (client.IsUserAuthorized())
            {
                return;
            }
            await Request();
           await  AuthUser();
        }
      
        private static async Task<string> Request()
        {
            await client.ConnectAsync();
            Hash = await client.SendCodeRequestAsync("");
            return Hash;
        }

        private static async Task AuthUser()
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                Thread.Sleep(10000);
                await AuthUser();
            }
            await client.ConnectAsync();
            TLUser user = null;
            try
            {
                user = await client.MakeAuthAsync("", Hash, Code);
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
                throw new Exception("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",ex);
            }
        }
        public static async Task SendMessageToChannelTest(string message)
        {
            await client.ConnectAsync();
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var chat = dialogs.chats.lists
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.title.ToLower() == "ptp");
            await client.SendMessageAsync(new TLInputPeerChannel() { channel_id = chat.id, access_hash = chat.access_hash.Value }, message);
        }

        public static async Task<bool> CheckNumber(string number)
        {
            await client.ConnectAsync();
            return await client.IsPhoneRegisteredAsync(number);

        }
        public static async Task SendMessageTest(string message, string number = null)
        {
            
            // this is because the contacts in the address come without the "+" prefix
            var normalizedNumber = number??"";

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
    }
}
