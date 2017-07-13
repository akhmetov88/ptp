using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using NLog;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Enums;
using TradingPlatform.Models.PageModel;

namespace TradingPlatform.Models
{
    public class StaticData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<HelpGroupViewModel> FAQ = new List<HelpGroupViewModel>();
        public static List<Currency> Currencies = new List<Currency>();
        public static List<Notification> Notifications = new List<Notification>();
        public static List<Trade> TradesToClose = new List<Trade>();
        private static readonly StaticData _instance = new StaticData();

        private ApplicationDbContext db = ApplicationDbContext.Create();
        

        //public List<CatalogModel> Catalogs => GetCatalogModels();
      //  public List<CatalogModel> CatalogsUpdated => GetCatalogModels(db, true);
        public static StaticData Inst
        {
            get { return _instance; }
        }

        public StaticData()
        {
            Currencies = GetCurrenciesFromNb();
            ReloadFAQ();
        }
        #region directory

        public void Update(ApplicationDbContext ctx)
        {
            _catalogModels = new List<CatalogModel>();
            _catalogs = new List<Catalog>();
            _catalogs.AddRange(ctx.Catalogs.ToList());
            _catalogModels.AddRange(_catalogs.Where(c => !c.ParentId.HasValue).Select(m => new CatalogModel(m)).ToList());
        }
        public List<Catalog> GetAllCatalogsList()
        {           
            return _catalogs?? (_catalogs=db.Catalogs.ToList());
        }

        private List<Catalog> _catalogs { get; set; }
        

        private List<CatalogModel> _catalogModels { get; set; }

        public List<CatalogModel> GetCatalogModels()
        {
            //if ((_catalogModels != null && _catalogModels.Any()))
                return _catalogModels?? GetAllCatalogsList().Where(c => !c.ParentId.HasValue && c.IsUsable).Select(m => new CatalogModel(m)).ToList();
        }



        public List<Currency> GetCurrenciesFromNb()
        {
            try
            {
                if (Currencies.Any())
                {
                    TimeSpan latUpdate = DateTime.Now - DateTime.Parse(Currencies.FirstOrDefault()?.DateOfRate);
                    if (latUpdate.TotalHours < 1)
                    {
                        logger.Info("currencies not updated, it was less one hour ");
                        return Currencies;
                    }
                }
                string url = "http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var allcurrencies = JsonConvert.DeserializeObject<IEnumerable<Currency>>(responseFromServer);
                var neededcurrencies = new List<string>(3) { "EUR", "USD", "RUB" };
                reader.Close();
                dataStream.Close();
                response.Close();
                logger.Info("CURRENCIES UPDATE AT {0}", DateTime.UtcNow);
                return allcurrencies.Where(c => neededcurrencies.Contains(c.Abbrreviature)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new List<Currency>() {
                    new Currency()
                    {
                        Abbrreviature = "USD",
                        CurrencyCode = 192,
                        CurrencyName = "",
                        Rate = 0m
                    },
                    new Currency()
                    {
                        Abbrreviature = "EUR",
                        CurrencyCode = 192,
                        CurrencyName = "",
                        Rate = 0m
                    },
                    new Currency()
                    {
                        Abbrreviature = "RUB",
                        CurrencyCode = 192,
                        CurrencyName = "",
                        Rate = 0m
                    } };
            }

        }


        public string GetCatalogValue(string code)
        {
            try
            {
                var locale = LocalText.GetCultureTwoLetter();
                if (String.IsNullOrEmpty(locale))
                    locale = "uk";

                    if (String.IsNullOrWhiteSpace(code))
                        code = "uk";
                var t = GetAllCatalogsList().FirstOrDefault(c => c.Code == code);
                    return  locale== "ru" ? t?.Ru??"" : t?.Uk??"";

                
            }
            catch (Exception ex)
            {
                logger.Error(ex, code);
                return string.Empty;
            }

        }
        public CatalogModel GetCatalogModel( string codeOfCatalog)
        {
            try
            {
                return new CatalogModel(GetAllCatalogsList().FirstOrDefault(c => c.Code == codeOfCatalog));
            }
            catch (Exception ex)
            {
                logger.Error(ex, codeOfCatalog);
                return new CatalogModel();
            }

        }
        public string GetCatalogDesc(string code,string locale="uk")
        {
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    if (LocalText.GetCultureTwoLetter() != null)
                        locale = LocalText.GetCultureTwoLetter();
                    return locale == "ru" ? _catalogs.FirstOrDefault(c => c.Code == code)?.DescRu ?? "" : _catalogs.FirstOrDefault(c => c.Code == code)?.DescUk ?? "";
                }
                else return "";
                
            }
            catch (Exception ex)
            {
                logger.Error(ex, code);
                return string.Empty;
            }

        }


        #endregion

        #region faq
        public List<HelpClass> GetAllHelps()
        {
            string Lang = LocalText.GetCultureTwoLetter();
            if (Lang == "uk")
                return FAQ.Select(x => new HelpClass()
                {
                    Hashtag = x.Hashtag,
                    Title = x.TitleUa,
                    Value = x.Ua,
                    Helps = x.Helps.Select(y => new HelpClass()
                    {
                        Hashtag = y.Hashtag,
                        Title = y.TitleUa,
                        Value = y.Ua,
                    }).ToList()

                }).ToList();
            else if (Lang == "ru")
                return FAQ.Select(x => new HelpClass()
                {
                    Hashtag = x.Hashtag,
                    Title = x.TitleRu,
                    Value = x.Ru,
                    Helps = x.Helps.Select(y => new HelpClass()
                    {
                        Hashtag = y.Hashtag,
                        Title = y.TitleRu,
                        Value = y.Ru,
                    }).ToList()

                }).ToList();
            else
                return FAQ.Select(x => new HelpClass()
                {
                    Hashtag = x.Hashtag,
                    Title = x.TitleEn,
                    Value = x.En,
                    Helps = x.Helps.Select(y => new HelpClass()
                    {
                        Hashtag = y.Hashtag,
                        Title = y.TitleEn,
                        Value = y.En,
                    }).ToList()

                }).ToList();
        }

        public List<HelpGroupViewModel> GetHelpGroups(int? id = null)
        {
            using (var db = new ApplicationDbContext())
            {
                var dl = new FAQDataLayer();
                if (id.HasValue && id.Value > 0)
                    return db.HelpGroups.Where(x => x.Id == id.Value).ToList().Select(hg => dl.FillHelpGroupViewModel(hg)).OrderBy(h => h.OrderId).ToList();
                else
                    return db.HelpGroups.ToList().Select(hg => dl.FillHelpGroupViewModel(hg)).OrderBy(h => h.OrderId).ToList();
            };

        }

        public void ReloadFAQ()
        {
            FAQ = GetHelpGroups();
        }
        #endregion
    }
}
