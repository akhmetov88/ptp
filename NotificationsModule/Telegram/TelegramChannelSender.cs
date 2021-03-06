﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Network;
using TLSharp.Core.Utils;

namespace NotificationsModule.Telegram
{
    public class TelegramChannelSender
    {
        public TelegramChannelSender(string code)
        {

        }
        private string NumberToSendMessage { get; set; }

        private string NumberToAuthenticate { get; set; }

        private string CodeToAuthenticate { get; set; }

        private string PasswordToAuthenticate { get; set; }

        private string NotRegisteredNumberToSignUp { get; set; }

        private string UserNameToSendMessage { get; set; }

        private string NumberToGetUserFull { get; set; }

        private string NumberToAddToChat { get; set; }

        private string ApiHash { get; set; }

        private int ApiId { get; set; }
        


        public void Init()
        {
            // Setup your API settings and phone numbers in app.config
            GatherTestConfiguration();
        }
        public class WebSessionStore : ISessionStore
        {
            public void Save(Session session)
            {
                var file = HttpContext.Current.Server.MapPath("/Content/docs/{0}.dat");

                using (FileStream fileStream = new FileStream(string.Format(file, (object)session.SessionUserId), FileMode.OpenOrCreate))
                {
                    byte[] bytes = session.ToBytes();
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }

            public Session Load(string sessionUserId)
            {
                var file = HttpContext.Current.Server.MapPath("/Content/docs/{0}.dat");

                string path = string.Format(file, (object)sessionUserId);
                if (!File.Exists(path))
                    return (Session)null;

                var buffer = File.ReadAllBytes(path);
                return Session.FromBytes(buffer, this, sessionUserId);
            }
        }
        private TelegramClient NewClient()
        {
            try
            {
                return new TelegramClient(ApiId, ApiHash, new WebSessionStore());
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception($"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
                                    ex);
            }
        }

        public void GatherTestConfiguration()
        {
            string appConfigMsgWarning = "{0} not configured in app.config! Some tests may fail.";

            ApiHash = ConfigurationManager.AppSettings[nameof(ApiHash)];
            if (string.IsNullOrEmpty(ApiHash))
                Debug.WriteLine(appConfigMsgWarning, nameof(ApiHash));

            var apiId = ConfigurationManager.AppSettings[nameof(ApiId)];
            if (string.IsNullOrEmpty(apiId))
                Debug.WriteLine(appConfigMsgWarning, nameof(ApiId));
            else
                ApiId = int.Parse(apiId);

            NumberToAuthenticate = ConfigurationManager.AppSettings[nameof(NumberToAuthenticate)];
            if (string.IsNullOrEmpty(NumberToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(NumberToAuthenticate));

            CodeToAuthenticate = ConfigurationManager.AppSettings[nameof(CodeToAuthenticate)];
            if (string.IsNullOrEmpty(CodeToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(CodeToAuthenticate));

            PasswordToAuthenticate = ConfigurationManager.AppSettings[nameof(PasswordToAuthenticate)];
            if (string.IsNullOrEmpty(PasswordToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(PasswordToAuthenticate));

            NotRegisteredNumberToSignUp = ConfigurationManager.AppSettings[nameof(NotRegisteredNumberToSignUp)];
            if (string.IsNullOrEmpty(NotRegisteredNumberToSignUp))
                Debug.WriteLine(appConfigMsgWarning, nameof(NotRegisteredNumberToSignUp));

            UserNameToSendMessage = ConfigurationManager.AppSettings[nameof(UserNameToSendMessage)];
            if (string.IsNullOrEmpty(UserNameToSendMessage))
                Debug.WriteLine(appConfigMsgWarning, nameof(UserNameToSendMessage));

            NumberToGetUserFull = ConfigurationManager.AppSettings[nameof(NumberToGetUserFull)];
            if (string.IsNullOrEmpty(NumberToGetUserFull))
                Debug.WriteLine(appConfigMsgWarning, nameof(NumberToGetUserFull));

            NumberToAddToChat = ConfigurationManager.AppSettings[nameof(NumberToAddToChat)];
            if (string.IsNullOrEmpty(NumberToAddToChat))
                Debug.WriteLine(appConfigMsgWarning, nameof(NumberToAddToChat));
        }

        public virtual async Task AuthUser()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var hash = await client.SendCodeRequestAsync(NumberToAuthenticate);

            var code = CodeToAuthenticate; // you can change code in debugger too

            if (String.IsNullOrWhiteSpace(code))
            {
                throw new Exception("CodeToAuthenticate is empty in the app.config file, fill it with the code you just got now by SMS/Telegram");
            }

            TLUser user = null;
            try
            {
                user = await client.MakeAuthAsync(NumberToAuthenticate, hash, code);
            }
            catch (CloudPasswordNeededException ex)
            {
                var password = await client.GetPasswordSetting();
                var password_str = PasswordToAuthenticate;

                user = await client.MakeAuthWithPasswordAsync(password, password_str);
            }
            catch (InvalidPhoneCodeException ex)
            {
                throw new Exception("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",
                                    ex);
            }
            
        }

        public virtual async Task SendMessageTest()
        {
            NumberToSendMessage = ConfigurationManager.AppSettings[nameof(NumberToSendMessage)];
            if (string.IsNullOrWhiteSpace(NumberToSendMessage))
                throw new Exception($"Please fill the '{nameof(NumberToSendMessage)}' setting in app.config file first");

            // this is because the contacts in the address come without the "+" prefix
            var normalizedNumber = NumberToSendMessage.StartsWith("+") ?
                NumberToSendMessage.Substring(1, NumberToSendMessage.Length - 1) :
                NumberToSendMessage;

            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.GetContactsAsync();

            var user = result.users.lists
                .OfType<TLUser>()
                .FirstOrDefault(x => x.phone == normalizedNumber);

            if (user == null)
            {
                throw new System.Exception("Number was not found in Contacts List of user: " + NumberToSendMessage);
            }

            await client.SendTypingAsync(new TLInputPeerUser() { user_id = user.id });
            Thread.Sleep(3000);
            await client.SendMessageAsync(new TLInputPeerUser() { user_id = user.id }, "TEST");
        }

        public virtual async Task SendMessageToChannelTest()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var chat = dialogs.chats.lists
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.title == "TestGroup");

            await client.SendMessageAsync(new TLInputPeerChannel() { channel_id = chat.id, access_hash = chat.access_hash.Value }, "TEST MSG");
        }

        public virtual async Task SendPhotoToContactTest()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.GetContactsAsync();

            var user = result.users.lists
                .OfType<TLUser>()
                .FirstOrDefault(x => x.phone == NumberToSendMessage);

            var fileResult = (TLInputFile)await client.UploadFile("cat.jpg", new StreamReader("data/cat.jpg"));
            await client.SendUploadedPhoto(new TLInputPeerUser() { user_id = user.id }, fileResult, "kitty");
        }

        public virtual async Task SendBigFileToContactTest()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.GetContactsAsync();

            var user = result.users.lists
                .OfType<TLUser>()
                .FirstOrDefault(x => x.phone == NumberToSendMessage);

            var fileResult = (TLInputFileBig)await client.UploadFile("some.zip", new StreamReader("<some big file path>"));

            await client.SendUploadedDocument(
                new TLInputPeerUser() { user_id = user.id },
                fileResult,
                "some zips",
                "application/zip",
                new TLVector<TLAbsDocumentAttribute>());
        }

        public virtual async Task DownloadFileFromContactTest()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.GetContactsAsync();

            var user = result.users.lists
                .OfType<TLUser>()
                .FirstOrDefault(x => x.phone == NumberToSendMessage);

            var inputPeer = new TLInputPeerUser() { user_id = user.id };
            var res = await client.SendRequestAsync<TLMessagesSlice>(new TLRequestGetHistory() { peer = inputPeer });
            var document = res.messages.lists
                .OfType<TLMessage>()
                .Where(m => m.media != null)
                .Select(m => m.media)
                .OfType<TLMessageMediaDocument>()
                .Select(md => md.document)
                .OfType<TLDocument>()
                .First();

            var resFile = await client.GetFile(
                new TLInputDocumentFileLocation()
                {
                    access_hash = document.access_hash,
                    id = document.id,
                    version = document.version
                },
                document.size);

        
        }

        public virtual async Task DownloadFileFromWrongLocationTest()
        {
            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.GetContactsAsync();

            var user = result.users.lists
                .OfType<TLUser>()
                .FirstOrDefault(x => x.id == 5880094);

            var photo = ((TLUserProfilePhoto)user.photo);
            var photoLocation = (TLFileLocation)photo.photo_big;

            var resFile = await client.GetFile(new TLInputFileLocation()
            {
                local_id = photoLocation.local_id,
                secret = photoLocation.secret,
                volume_id = photoLocation.volume_id
            }, 1024);

            var res = await client.GetUserDialogsAsync();

          
        }

        public virtual async Task SignUpNewUser()
        {
            var client = NewClient();
            await client.ConnectAsync();

            var hash = await client.SendCodeRequestAsync(NotRegisteredNumberToSignUp);
            var code = "";

            var registeredUser = await client.SignUpAsync(NotRegisteredNumberToSignUp, hash, code, "TLSharp", "User");
          

            var loggedInUser = await client.MakeAuthAsync(NotRegisteredNumberToSignUp, hash, code);
      
        }

        public virtual async Task CheckPhones()
        {
            var client = NewClient();
            await client.ConnectAsync();

            var result = await client.IsPhoneRegisteredAsync(NumberToAuthenticate);
           
        }

        public virtual async Task FloodExceptionShouldNotCauseCannotReadPackageLengthError()
        {
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    await CheckPhones();
                }
                catch (FloodException floodException)
                {
                    Console.WriteLine($"FLOODEXCEPTION: {floodException}");
                    Thread.Sleep(floodException.TimeToWait);
                }
            }
        }

        public virtual async Task SendMessageByUserNameTest()
        {
            UserNameToSendMessage = ConfigurationManager.AppSettings[nameof(UserNameToSendMessage)];
            if (string.IsNullOrWhiteSpace(UserNameToSendMessage))
                throw new Exception($"Please fill the '{nameof(UserNameToSendMessage)}' setting in app.config file first");

            var client = NewClient();

            await client.ConnectAsync();

            var result = await client.SearchUserAsync(UserNameToSendMessage);

            var user = result.users.lists
                .Where(x => x.GetType() == typeof(TLUser))
                .OfType<TLUser>()
                .FirstOrDefault(x => x.username == UserNameToSendMessage.TrimStart('@'));

            if (user == null)
            {
                var contacts = await client.GetContactsAsync();

                user = contacts.users.lists
                    .Where(x => x.GetType() == typeof(TLUser))
                    .OfType<TLUser>()
                    .FirstOrDefault(x => x.username == UserNameToSendMessage.TrimStart('@'));
            }

            if (user == null)
            {
                throw new System.Exception("Username was not found: " + UserNameToSendMessage);
            }

            await client.SendTypingAsync(new TLInputPeerUser() { user_id = user.id });
            Thread.Sleep(3000);
            await client.SendMessageAsync(new TLInputPeerUser() { user_id = user.id }, "TEST");
        }
    }
}

