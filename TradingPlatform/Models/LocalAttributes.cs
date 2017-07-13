using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace TradingPlatform
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // The user is not authenticated
                base.HandleUnauthorizedRequest(filterContext);
            }
            else if (!this.Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
            {
                // The user is not in any of the listed roles => 
                // show the unauthorized view
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Unauthorized.cshtml"
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }


    public class LDisplayNameAttribute : DisplayNameAttribute
    {
        private string cult = LocalText.GetCultureName();
        private string Scope { get; set; }
        private string Key { get; set; }
        private string DefaultUk { get; set; }
        private string DefaultRu { get; set; }

        public LDisplayNameAttribute(string scope, string key, string defaultUk = "", string defaultRu = "")
        {
            this.DefaultRu = defaultRu;
            this.DefaultUk = defaultUk;
            this.Key = key;
            this.Scope = scope;
        }
        
        public override string DisplayName
        {
            get
            {
                return LocalText.Inst.Get(Scope, Key, DefaultUk, DefaultRu, LocalText.GetCultureName());
            }
        }
    }
    public class RequiredLocalizedAttribute : RequiredAttribute, IClientValidatable
    {
        private string Scope { get; set; }
        private string Key { get; set; }
        private string DefaultUk { get; set; }
        private string DefaultRu { get; set; }

        public RequiredLocalizedAttribute(string scope, string key, string defaultUk = "", string defaultRu = "")
        {
            this.DefaultRu = defaultRu;
            this.DefaultUk = defaultUk;
            this.Key = key;
            this.Scope = scope;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty((string) value))
            {
                return new ValidationResult(FormatErrorMessage(""));
            }
            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
          //  return base.FormatErrorMessage(name);
          return LocalText.Inst.Get(Scope, Key, DefaultUk, DefaultRu);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "require"
            };
            yield return clientValidationRule;
        }
    }


    




}