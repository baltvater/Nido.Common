using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Exceptions
{
    /// <summary>
    /// Any repeatable exception will be assign to this type.
    /// This indicated a data related error so user can repeat the operation upon correcting the input data.
    /// </summary>
    public class ExceptionRepeatable : ExceptionBase
    {
        /// <summary>
        /// User define contructor
        /// </summary>
        /// <param name="ex">Actual exception occurred</param>
        public ExceptionRepeatable(Exception ex)
            : base(ex)
        {
            this.ExceptionType = ExceptionTypes.Repeatable;
            UserErrorMessage = "An error occured while performing the operation. Please try again after few minutes.";
        }
    }
}
