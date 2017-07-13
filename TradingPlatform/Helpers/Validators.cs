using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Enums;

namespace TradingPlatform.Helpers
{
    public class FileUploadValidator: ValidationAttribute, IClientValidatable
    {
        private MinimumFileSizeValidator _minimumFileSizeValidator;
        private MaximumFileSizeValidator _maximumFileSizeValidator;
        private ValidFileTypeValidator _validFileTypeValidator;

        /// <param name="validFileTypes">Valid file extentions(without the dot)</param>
        public FileUploadValidator(params string[] validFileTypes)
            : base()
        {
            _validFileTypeValidator = new ValidFileTypeValidator(validFileTypes);
        }

        /// <param name="maximumFileSize">Maximum file size in MB</param>
        /// <param name="validFileTypes">Valid file extentions(without the dot)</param>
        public FileUploadValidator(double maximumFileSize, params string[] validFileTypes)
            : base()
        {
            _maximumFileSizeValidator = new MaximumFileSizeValidator(maximumFileSize);
            _validFileTypeValidator = new ValidFileTypeValidator(validFileTypes);
        }

        /// <param name="minimumFileSize">MinimumFileSize file size in MB</param>
        /// <param name="maximumFileSize">Maximum file size in MB</param>
        /// <param name="validFileTypes">Valid file extentions(without the dot)</param>
        public FileUploadValidator(double minimumFileSize, double maximumFileSize, params string[] validFileTypes)
            : base()
        {
            _minimumFileSizeValidator = new MinimumFileSizeValidator(minimumFileSize);
            _maximumFileSizeValidator = new MaximumFileSizeValidator(maximumFileSize);
            _validFileTypeValidator = new ValidFileTypeValidator(validFileTypes);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            try
            {
                if (value.GetType() != typeof(HttpPostedFileWrapper))
                {
                    throw new InvalidOperationException("");
                }

                var errorMessage = new StringBuilder();
                var file = value as HttpPostedFileBase;

                if (_minimumFileSizeValidator != null)
                {
                    if (!_minimumFileSizeValidator.IsValid(file))
                    {
                        errorMessage.Append(String.Format("{0}. ", _minimumFileSizeValidator.FormatErrorMessage(validationContext.DisplayName)));
                    }
                }

                if (_maximumFileSizeValidator != null)
                {
                    if (!_maximumFileSizeValidator.IsValid(file))
                    {
                        errorMessage.Append(String.Format("{0}. ", _maximumFileSizeValidator.FormatErrorMessage(validationContext.DisplayName)));
                    }
                }

                if (_validFileTypeValidator != null)
                {
                    if (!_validFileTypeValidator.IsValid(file))
                    {
                        errorMessage.Append(String.Format("{0}. ", _validFileTypeValidator.FormatErrorMessage(validationContext.DisplayName)));
                    }
                }

                if (String.IsNullOrEmpty(errorMessage.ToString()))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(errorMessage.ToString());
                }
            }
            catch (Exception excp)
            {
                return new ValidationResult(excp.Message);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "fileuploadvalidator"
            };

            var clientvalidationmethods = new List<string>();
            var parameters = new List<string>();
            var errorMessages = new List<string>();

            if (_minimumFileSizeValidator != null)
            {
                clientvalidationmethods.Add(_minimumFileSizeValidator.GetClientValidationRules(metadata, context).First().ValidationType);
                parameters.Add(_minimumFileSizeValidator.MinimumFileSize.ToString());
                errorMessages.Add(_minimumFileSizeValidator.FormatErrorMessage(metadata.GetDisplayName()));
            }

            if (_maximumFileSizeValidator != null)
            {
                clientvalidationmethods.Add(_maximumFileSizeValidator.GetClientValidationRules(metadata, context).First().ValidationType);
                parameters.Add(_maximumFileSizeValidator.MaximumFileSize.ToString());
                errorMessages.Add(_maximumFileSizeValidator.FormatErrorMessage(metadata.GetDisplayName()));
            }

            if (_validFileTypeValidator != null)
            {
                clientvalidationmethods.Add(_validFileTypeValidator.GetClientValidationRules(metadata, context).First().ValidationType);
                parameters.Add(String.Join(",", _validFileTypeValidator.ValidFileTypes));
                errorMessages.Add(_validFileTypeValidator.FormatErrorMessage(metadata.GetDisplayName()));
            }

            clientValidationRule.ValidationParameters.Add("clientvalidationmethods", clientvalidationmethods.ToConcatenatedString(","));
            clientValidationRule.ValidationParameters.Add("parameters", parameters.ToConcatenatedString("|"));
            clientValidationRule.ValidationParameters.Add("errormessages", errorMessages.ToConcatenatedString(","));

            yield return clientValidationRule;
        }

        private double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
    public class MaximumFileSizeValidator
       : ValidationAttribute, IClientValidatable
    {
        private string _errorMessage = LocalText.Inst.Get("error", "fileTooBigErrorr","Файл не повинен бути більше {0} Mб", "Файл не должен быть больше {0} Mб");

        /// <summary>
        /// Maximum file size in MB
        /// </summary>
        public double MaximumFileSize { get; private set; }

        /// <param name="maximumFileSize">Maximum file size in MB</param>
        public MaximumFileSizeValidator(double maximumFileSize)
        {
            MaximumFileSize = maximumFileSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!IsValidMaximumFileSize((value as HttpPostedFileBase).ContentLength))
            {
                ErrorMessage = String.Format(_errorMessage, MaximumFileSize);
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(_errorMessage, name, MaximumFileSize);
        }

        public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "maximumfilesize"
            };

            clientValidationRule.ValidationParameters.Add("size", MaximumFileSize);

            return new[] { clientValidationRule };
        }

        private bool IsValidMaximumFileSize(
            int fileSize)
        {
            return (ConvertBytesToMegabytes(fileSize) <= MaximumFileSize);
        }

        private double ConvertBytesToMegabytes(
           int bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }

    public class MinimumFileSizeValidator: ValidationAttribute, IClientValidatable
    {
        private string _errorMessage = "{0} can not be smaller than {1} MB";

        /// <summary>
        /// Minimum file size in MB
        /// </summary>
        public double MinimumFileSize { get; private set; }

        /// <param name="minimumFileSize">MinimumFileSize file size in MB</param>
        public MinimumFileSizeValidator(
           double minimumFileSize)
            : base()
        {
            MinimumFileSize = minimumFileSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!IsValidMinimumFileSize((value as HttpPostedFileBase).ContentLength))
            {
                ErrorMessage = String.Format(_errorMessage, "{0}", MinimumFileSize);
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(_errorMessage, name, MinimumFileSize);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "minimumfilesize"
            };

            clientValidationRule.ValidationParameters.Add("size", MinimumFileSize);

            return new[] { clientValidationRule };
        }

        private bool IsValidMinimumFileSize(
            int fileSize)
        {
            return ConvertBytesToMegabytes(fileSize) >= MinimumFileSize;
        }

        private double ConvertBytesToMegabytes(int bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }


    public class ValidFileTypeValidator: ValidationAttribute, IClientValidatable
    {
        private string _errorMessage = LocalText.Inst.Get("error", "fileFormatError", "Файл повинен бути одного з цих форматів: {0}", "Файл должен быть одного из этих форматов: {0}");

        /// <summary>
        /// Valid file extentions
        /// </summary>
        public string[] ValidFileTypes { get; private set; }

        /// <param name="validFileTypes">Valid file extentions(without the dot)</param>
        public ValidFileTypeValidator(params string[] validFileTypes)
        {
            ValidFileTypes = validFileTypes;
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;

            if (value == null || String.IsNullOrEmpty(file.FileName))
            {
                return true;
            }

            if (ValidFileTypes != null)
            {
                var validFileTypeFound = false;

                foreach (var validFileType in ValidFileTypes)
                {
                    var fileNameParts = file.FileName.Split('.');
                    
                    if (String.Equals(fileNameParts[fileNameParts.Length - 1].ToLower(), validFileType.ToLower(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        validFileTypeFound = true;
                        break;
                    }
                }

                if (!validFileTypeFound)
                {
                    ErrorMessage = _errorMessage.Replace("{0}", ValidFileTypes.ToConcatenatedString(","));//String.Format(_errorMessage, ValidFileTypes.ToConcatenatedString(","));
                    return false;
                }
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return _errorMessage.Replace("{0}", ValidFileTypes.ToConcatenatedString(",")); //String.Format(_errorMessage, name, ValidFileTypes.ToConcatenatedString(","));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "validfiletype"
            };

            clientValidationRule.ValidationParameters.Add("filetypes", ValidFileTypes.ToConcatenatedString(","));

            return new[] { clientValidationRule };
        }
    }
}
