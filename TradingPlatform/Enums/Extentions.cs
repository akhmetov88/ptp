using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using TradingPlatform.Data;
using TradingPlatform.Models;
using System.Threading.Tasks;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Enums
{
    public static class Extentions
    {      

        public static List<List<T>> CustomSplit<T>(this List<T> source)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 1000)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

                   

        public static string ToClientDate(this DateTime dt)
        {
            // read the value from session
            var timeOffSet = HttpContext.Current.Session["timezoneoffset"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dt = dt.AddMinutes(-1 * offset);

                return dt.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo(LocalText.GetCultureTwoLetter()));
            }
#warning хардкод даты приведения
            dt = dt.AddMinutes(-1 * -180);
            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime().ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo(LocalText.GetCultureTwoLetter()));
        }

        public static string ToClientTime(this DateTime dt)
        {
            // read the value from session
            var timeOffSet = HttpContext.Current.Session["timezoneoffset"]??-180;
          //  var timediff = HttpContext.Current.Session["timediff"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                //var millisecondsdiff = double.Parse(timediff.ToString());
                dt = dt.AddMinutes(-1 * offset);

                return dt.ToString("g", CultureInfo.GetCultureInfo(LocalText.GetCultureTwoLetter()));
            }
            dt = dt.AddMinutes(-1 * -180);
            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime().ToString("g", CultureInfo.GetCultureInfo(LocalText.GetCultureTwoLetter()));
        }

        public static DateTime ToClientTimeDate(this DateTime dt)
        {

           // read the value from session
           var timeOffSet = HttpContext.Current?.Session["timezoneoffset"] ?? -180;
            var timediff = HttpContext.Current?.Session["timediff"] ?? 0;

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
              //  var millisecondsdiff = double.Parse(timediff.ToString());
                dt = dt.AddMinutes(-1 * offset);

                return dt;
            }         
            // if there is no offset in session return the datetime in server timezone
            return dt.AddHours(3);
        }


        public static string ToServerTicks(this DateTime dt)
        {
            // read the value from session
            var timeOffSet = HttpContext.Current.Session["timezoneoffset"]??-180;
            var timediff = HttpContext.Current?.Session["timediff"]??0;

            if (timeOffSet != null && timediff != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                var millisecondsdiff = double.Parse(timediff.ToString());
                dt = dt.AddMinutes(-1 * offset).AddMilliseconds(millisecondsdiff);
                return
                    dt.ToString("yyyy-MM-dd HH:mm:ss");
                //  return dt.ToString("g", CultureInfo.GetCultureInfo(LocalText.GetCultureTwoLetter()));
            }
            dt = dt.AddMinutes(-1 * -180);
            // if there is no offset in session return the datetime in server timezone
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToParse(this DateTime dt)
        {
            // read the value from session
            var timeOffSet = HttpContext.Current.Session["timezoneoffset"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dt = dt.AddMinutes(-1 * offset).AddSeconds(1);
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            dt = dt.AddMinutes(-1 * -180).AddSeconds(1);
            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static long ToInteger(this double val)
        {
            try
            {
                return (long)val;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ToStandartString(this decimal val)
        {
            return val.ToString("C",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 2
                });
        }

        /// <summary>
        /// Возвращает бизнес-группу пользователя для
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserGroup(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserGroup");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        /// <summary>
        /// Возвращает набор айдишников юридических лиц под управлением текущего пользователя
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>идентификаторы контрагентов</returns>
        public static List<int> GetUserContragents(this IIdentity identity)
        {
            var contragentClaim = ((ClaimsIdentity)identity).FindFirst("UserContragents");
            if (!string.IsNullOrEmpty(contragentClaim.Value))
                return contragentClaim.Value.Split(',').Select(Int32.Parse).ToList();
            else
                return new List<int>();
        }

        public static bool IsRegisteredToTrade(this IIdentity identity, List<int> thisTradeContragents)
        {
            return thisTradeContragents.Intersect(identity.GetUserContragents()).Any();

        }
      
        public static string GetUserNamen(this IIdentity identity, ApplicationDbContext db)
        {
            
                var user = db.Users.Find(identity.GetUserId());
                if (user==null)
                {
                    user = db.Users.SingleOrDefault(c => c.Email == identity.Name);
                }
                return user.RegisterName ?? user.Email ?? "";
            
        }
        /// <summary>
        /// Возвращает, может ли текущий юзер ставить ставку на торгах, проверяются группы создателя торгов и текущего юзера
        /// </summary>
        /// <param name="identity">User.Identity</param>
        /// <param name="groupid">Группа, которая проверяется</param>
        /// <returns></returns>
        public static bool CurrentTradeUSerBet(this IIdentity identity, string groupid)
        {
            string usergroup = identity.GetUserGroup();
            return !string.IsNullOrEmpty(usergroup) && !groupid.Equals(usergroup);

        }

        public static string ToConcatenatedString(this List<string> list, string separator)
        {
            return String.Join(separator, list).ToLower();
        }

        public static string ToConcatenatedString(this string[] list, string separator)
        {
            return String.Join(separator, list).ToLower();
        }



    }
}



