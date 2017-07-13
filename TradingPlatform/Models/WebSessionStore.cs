using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TLSharp.Core;
using TradingPlatform.Messaging;

namespace TradingPlatform.Models
{
    public class WebSessionStore : ISessionStore
    {
        //public WebSessionStore()
        //{
        //    DeleteOldSession();
        //}
        public void Save(Session session)
        {
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Content/docs/{0}.dat");

            using (FileStream fileStream = new FileStream(string.Format(file, (object)session.SessionUserId), FileMode.OpenOrCreate))
            {
                byte[] bytes = session.ToBytes();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public Session Load(string sessionUserId)
        {
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Content/docs/{0}.dat");

            string path = string.Format(file, (object)sessionUserId);
            if (!System.IO.File.Exists(path))
                return (Session)null;

            var buffer = System.IO.File.ReadAllBytes(path);
            return Session.FromBytes(buffer, this, sessionUserId);
        }
        //private void DeleteOldSession()
        //{
        //    var file = System.Web.HttpContext.Current.Server.MapPath("~/Content/docs/session.dat");
        //    EmailFactory.SendEmail("admin@ptp.ua", "Файл сессии", file + " exists: " + System.IO.File.Exists(file));

        //    if (System.IO.File.Exists(file))
        //    {
        //        System.IO.File.Delete(file);
        //        // await Broadcast(model, "ptpuatest");
        //    }
        //    EmailFactory.SendEmail("admin@ptp.ua", "Файл сессии", file + "after delete exists: " + System.IO.File.Exists(file));

        //}
    }
}