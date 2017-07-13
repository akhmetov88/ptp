using System;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.ViewModel
{
    public class ContentView
    {
        public ContentView()
        {

        }
        public ContentView(Content c)
        {
            Id = c.Id;
            Alias = c.Alias ?? "";
            Lang = c.Lang ?? "";
            Title = c.Title ?? "";
            KeyWords = c.KeyWords ?? "";
            Descriptions = c.Descriptions ?? "";
            HtmlContent = c.HtmlContent ?? "";//c.HtmlContent;
            Updated = c.UpdateDate ?? c.Updated;
        }
        public int Id { get; set; }

        [LDisplayName("ContentView", "DisplayNameLang", "Мова", "Язык")]
        // [LRequired("code", "DefaultError", "Поле не повинно быти порожнім", "Поле не должно быть пустым")]
        public string Lang { get; set; }

        [LDisplayName("ContentView", "DisplayNameTitle", "Заголовок", "Заголовок")]
        public string Title { get; set; }

        [LDisplayName("ContentView", "DisplayNameKeyWords", "Ключові слова", "Ключевые слова")]
        public string KeyWords { get; set; }

        [LDisplayName("ContentView", "DisplayNameDescriptions", "Опис", "Описание")]
        public string Descriptions { get; set; }

        [LDisplayName("ContentView", "DisplayNameLang", "Адреса", "Адрес")]
        //  [LRequired("code", "DefaultError", "Поле не повинно быти порожнім", "Поле не должно быть пустым")]
        public string Alias { get; set; }

        [LDisplayName("ContentView", "DisplayNameHtmlContent", "Контент", "Контент")]
        public string HtmlContent { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class TokenModel
    {
        public TokenModel(Token token)
        {
            Id = token.Id;
            Key = token.Key;
            Uk = token.Uk;
            Ru = token.Ru;
            PageLink = token.PageLink;
            UpdateDate = token.UpdateDate;
        }
        public TokenModel() { }
        public int Id { get; set; }
        public string Key { get; set; }

        public string Uk { get; set; }

        public string Ru { get; set; }

        public string PageLink { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}