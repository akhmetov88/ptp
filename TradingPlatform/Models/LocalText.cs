using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NLog;
using TradingPlatform.Data;
using TradingPlatform.Models;

namespace TradingPlatform
{
    public sealed class LocalText
    {
        public static List<Token> Tokens = new List<Token>();
        private static readonly LocalText _instance = new LocalText();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static LocalText Inst
        {
            get { return _instance; }
        }

        private LocalText()
        {
            Tokens = GetRes();
        }

        /// <summary>
        /// Локализация. Возвращает текст на языке клиента.
        /// </summary>
        /// <param name="scope">значение из перечня: "btn", "link", "title", "text", "code"</param>
        /// <param name="key">Ключ в БД</param>
        /// <param name="defaultValueUk">Значение по умолчанию для uk-UA, если ключа нет в БД</param>
        /// <param name="defaultValueRu">Значение по умолчанию для ru-RU, если ключа нет в БД</param>
        /// <param name="culture"></param>
        /// <returns>Возвращает текст на языке клиента</returns>
        public string Get(string scope, string key, string defaultValueUk = "", string defaultValueRu = "", string culture="", string userId=null)
        {
            Token res = Tokens.FirstOrDefault(x => x.Key.ToLower() == (scope + "_" + key).ToLower());

            if (res == null)
            {
                if (string.IsNullOrEmpty(defaultValueUk) && string.IsNullOrEmpty(defaultValueRu))
                    defaultValueUk = defaultValueRu = scope + "_" + key;

                if (string.IsNullOrEmpty(defaultValueUk))
                    defaultValueUk = defaultValueRu;

                if (string.IsNullOrEmpty(defaultValueRu))
                    defaultValueRu = defaultValueUk;

                Set(scope + "_" + key, defaultValueUk, defaultValueRu, userId);
                return defaultValueUk;
            }
            if (String.IsNullOrEmpty(culture))
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("language");
                if (cookie == null)
                {
                    cookie = CreateCookie();
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    return res.Uk;
                }
                culture = cookie.Value;
                switch (cookie.Value)
                {
                    case "ru-RU": return res.Ru;
                    default: return res.Uk;
                }

              //  culture = "uk-UA";
               
            }

            if (!String.IsNullOrEmpty(culture))
                return culture=="ru-RU" ? res.Ru : res.Uk;
            return culture == "ru-RU" ? res.Ru : res.Uk;
        }

        private static List<Token> GetRes()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Tokens.ToList();
            };
        }


        public static bool Set(string key, string uk, string ru, string userid=null)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    if (!db.Tokens.Any(t => t.Key == key))
                    {
                        var token = db.Tokens.Create();
                        token.Key = key;
                        token.Uk = uk;
                        token.Ru = ru;
                        token.PageLink = HttpContext.Current.Request.RawUrl;
                        token.UserId = userid;
                        db.Tokens.Add(token);
                        db.SaveChanges();
                        logger.Info($"token {key}: {uk} setted by {userid}");

                    }
                };
                Tokens = GetRes();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool Update(int id, string key, string uk, string ru, string userid=null)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var item = db.Tokens.SingleOrDefault(t => t.Id == id);
                    if (item == null)
                        return Set(key, uk, ru,userid);
                    else
                    {
                        item.Uk = uk;
                        item.Ru = ru;
                        item.UserId = userid;
                        item.UpdateDate = DateTime.UtcNow;
                        db.UpdateEntity(item);
                        logger.Info($"token {id} {key}: {uk} updated by {userid}");
                        // db.SaveChanges();
                    }
                };
                Tokens = GetRes();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string DefaultCulture = "uk-UA";
        public static string DefaultCultureTwoLetter = "uk";

        public static string GetCultureTwoLetter()
        {
            try
            {
                HttpCookie cookie = null;
                if (HttpContext.Current!=null)
                {
                    cookie = HttpContext.Current.Request.Cookies.Get("language");
                }
                if (cookie == null)
                    return DefaultCultureTwoLetter;
                return CultureInfo.CreateSpecificCulture(cookie.Value).TwoLetterISOLanguageName;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return DefaultCultureTwoLetter;
            }
        }

        public static string GetCultureName()
        {
            if (HttpContext.Current != null)
            {
                HttpCookie cookie = HttpContext.Current?.Request?.Cookies?.Get("language");
                if (cookie == null)
                    return DefaultCulture;
                else
                    return cookie.Value;
            }
            return DefaultCulture;
        }

        public static HttpCookie CreateCookie()
        {
            HttpCookie cookie = new HttpCookie("language");
            cookie.HttpOnly = false;
            cookie.Value = DefaultCulture;
            cookie.Expires = DateTime.UtcNow.AddYears(1);
            return cookie;
        }
    }
}