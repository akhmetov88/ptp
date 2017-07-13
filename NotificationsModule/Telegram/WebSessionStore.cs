using System.IO;
using System.Web;
using TLSharp.Core;

namespace NotificationsModule.Telegram
{
    public class WebSessionStore : ISessionStore
    {
        public void Save(Session session)
        {
            var file = HttpContext.Current.Server.MapPath("~/App_Data/{0}.dat");

            using (FileStream fileStream = new FileStream(string.Format(file, (object)session.SessionUserId), FileMode.OpenOrCreate))
            {
                byte[] bytes = session.ToBytes();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public Session Load(string sessionUserId)
        {
            var file = HttpContext.Current.Server.MapPath("~/App_Data/{0}.dat");
            string path = string.Format(file, (object)sessionUserId);
            if (!File.Exists(path))
                return (Session)null;
            var buffer = File.ReadAllBytes(path);
            return Session.FromBytes(buffer, this, sessionUserId);
        }
    }
}
