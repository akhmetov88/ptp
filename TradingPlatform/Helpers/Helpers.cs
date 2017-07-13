using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Serialization;
using NLog;

namespace TradingPlatform.Helpers
{


    public static class Helpers
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        [XmlRoot(ElementName = "url", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
        public class Url
        {
            [XmlElement(ElementName = "loc", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
            public string Loc { get; set; }
            [XmlElement(ElementName = "lastmod", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
            public string Lastmod { get; set; }
            [XmlElement(ElementName = "changefreq", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
            public string Changefreq { get; set; }
            [XmlElement(ElementName = "priority", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
            public string Priority { get; set; }
        }

        [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
        public class Urlset
        {
            [XmlElement(ElementName = "url", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
            public List<Url> Url { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string SchemaLocation { get; set; }
        }

        public static MvcHtmlString CustomDropdownList(string name, IEnumerable<SelectListItem> data, string selectedValues,
            object htmlAttributes, object htmlDataAttributes)
        {
            List<string> _selectedValues = new List<string>();
            _selectedValues.Add(selectedValues);
            return CustomDropdownList(name, data, _selectedValues, htmlAttributes, htmlDataAttributes);
        }

        public static MvcHtmlString CustomDropdownList(string name, IEnumerable<SelectListItem> data, List<string> selectedValues, object htmlAttributes, object htmlDataAttributes)
        {
            TagBuilder dropdown = new TagBuilder("select");
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            StringBuilder options = new StringBuilder();
            foreach (var item in data)
            {
                if (selectedValues != null && selectedValues.Contains(item.Value))
                    options = options.Append("<option value='" + item.Value + "' selected='selected'>" + item.Text + "</option>");
                else
                    options = options.Append("<option value='" + item.Value + "'>" + item.Text + "</option>");
            }
            dropdown.InnerHtml = options.ToString();

            var htmlAttr = new RouteValueDictionary(htmlAttributes);
            var htmlData = new RouteValueDictionary(htmlDataAttributes);
            foreach (var attributes in htmlData)
            {
                htmlAttr.Add(string.Format("data-{0}", attributes.Key), attributes.Value);
            }

            dropdown.MergeAttributes(htmlAttr);
            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }

      
        public class DateTimeWithOffsetModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                DateTime dateTime;
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                logger.Info($"binding {value.AttemptedValue} to UTC");

                var isDate = DateTime.TryParse(value.AttemptedValue, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime);
                if (isDate)
                {
                    logger.Info($"binded {value} to {dateTime.ToUniversalTime()}");
                    return dateTime.ToUniversalTime();
                }
               
                return dateTime.ToLocalTime();
            }
        }

      

    }
}