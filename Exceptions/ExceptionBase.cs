using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common.BackEnd;

namespace Nido.Common.Exceptions
{
    /// <summary>
    /// Base exception object
    /// </summary>
    public abstract class ExceptionBase : Exception
    {
        /// <summary>
        /// Type of expceptions
        /// </summary>
        public enum ExceptionTypes
        {
            /// <summary>
            /// The critical type where auto recovery is not possible
            /// </summary>
            Critical,
            /// <summary>
            /// Cannot repeat but the support team can recover you from this error.
            /// </summary>
            NoneRepeatable,
            /// <summary>
            /// Retrying will help remove this error.
            /// </summary>
            Repeatable,
            /// <summary>
            /// Data related error. User need to check his/ her input data.
            /// </summary>
            DataError
        }

        /// <summary>
        /// User define custom constructor
        /// </summary>
        /// <param name="ex">Exception occured</param>
        protected ExceptionBase(Exception ex)
            : base(ex.Message, ex.InnerException)
        {
        }

        /// <summary>
        /// The custom message to be displayed for the user.
        /// </summary>
        public virtual string UserErrorMessage { get; set; }
        /// <summary>
        /// The reference ID of this error. The User message contain this ID.
        /// </summary>
        public virtual string RefId { get; set; }
        /// <summary>
        /// Type of the exception. 
        /// Base on this type the UI layer developer can decide how to handle the error.
        /// </summary>
        public ExceptionTypes ExceptionType { get; set; }

        /// <summary>
        /// Method that create the reference ID for this error.
        /// </summary>
        /// <returns>Reference ID</returns>
        public static string GetRefId()
        {
            return (new StringBuilder(DateTime.Now.Year)
                .Append(DateTime.Now.Month)
                .Append(DateTime.Now.Day)
                .Append(DateTime.Now.Hour)
                .Append(DateTime.Now.Minute)
                .Append(DateTime.Now.Millisecond)
                .Append("-").Append(new Random().Next(100))).Append(' ').ToString();
        }

        /// <summary>
        /// Get the entity name of the generic entity object
        /// </summary>
        /// <typeparam name="E">Type of the generic object to be used to retrieve.</typeparam>
        /// <param name="entity">entity object</param>
        /// <returns></returns>
        public static string GetName<E>(E entity)
            where E : BaseObject
        {
            if ((entity != null && string.IsNullOrEmpty(entity.DisplayName)) || (entity == null))
            {
                string s = typeof (E).Name;
                int i = s.LastIndexOf('.');
                if (i > 0)
                    s = s.Substring(i);
                return s;
            }
            else
                return entity.DisplayName;
        }
    }
}
