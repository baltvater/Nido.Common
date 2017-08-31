using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nido.Common.Utilities.Validators
{
    public class StringLengthAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
    {
        public StringLengthAttribute(int MaxLength)
            : base(MaxLength)
        {
            this.ErrorMessage = "The {0} value cannot exceed {1} characters.";
        }
        
        private String displayName;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.displayName = "'" + validationContext.DisplayName + "'";
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, displayName, MaximumLength);
        }
    }
}
