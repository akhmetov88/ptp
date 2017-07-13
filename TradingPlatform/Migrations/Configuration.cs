using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Models;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TradingPlatform.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TradingPlatform.Models.ApplicationDbContext context)
        {
            context.Roles.AddOrUpdate(c=>c.Name,
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "root"},
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "admin" },
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "contragent" },
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "byer" },
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "trader" },
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "lawyer" },
                new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "watcher" }
                );

          context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Tokens] ON");
            foreach (var model in GetTokenz())
            {
                if (context.Tokens.FirstOrDefault(c => c.Id == model.Id) == null)
                    context.Tokens.Add(new Token(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Tokens] OFF");
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Contents] ON");
            foreach (var model in GetContent())
            {
                if(context.ContentPages.FirstOrDefault(c=>c.Id==model.Id)==null)
                    context.ContentPages.Add(new Content(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Contents] OFF");
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Catalogs] ON");
            foreach (var model in GetCatalogs())
            {
                if (context.Catalogs.FirstOrDefault(c => c.Id == model.Id) == null)
                    context.Catalogs.Add(new Catalog(model));
            }
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Catalogs] OFF");
        }

        private List<TokenModel> GetTokenz()
        {
            using (WebClient wc = new WebClient() {Encoding = Encoding.UTF8})
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/tokens");
                var models = JsonConvert.DeserializeObject<IEnumerable<TokenModel>>(data);
                return models.ToList();
            }
        }
        private List<ContentView> GetContent()
        {
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/contents");
                var models = JsonConvert.DeserializeObject<IEnumerable<ContentView>>(data);
                return models.ToList();
            }
        }
        private List<CatalogModel> GetCatalogs()
        {
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.DownloadString($"https://p-t-p-dev.azurewebsites.net/api/catalogs");
                var models = JsonConvert.DeserializeObject<IEnumerable<CatalogModel>>(data);
                return models.OrderBy(c=>c.Id).ToList();
            }
        }
    }
}
