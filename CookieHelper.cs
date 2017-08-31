using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Nido.Common
{
    /// <summary>
    /// All cookie related operation of the project
    /// needs to run through this class.
    /// </summary>    
    [Obsolete("Please use 'CookieHelper' class instead of this one")]
    public static class PortalCookie
    {
        /// <summary>
        /// Check if the cookie value is null
        /// </summary>
        /// <param name="Name">name of the cookie variable</param>
        /// <returns>true if not null or false if null</returns>
        public static bool IsNull(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
                return (HttpContext.Current.Request.Cookies[Name] != null);
            else
                return false;
        }

        /// <summary>
        /// Get the value of the cookie variable
        /// </summary>
        /// <param name="Name">name of the cookie</param>
        /// <param name="Key">key of the cookie variable.
        /// You can have multiple varialbe stored in a one cookie.</param>
        /// <returns></returns>
        public static string Get(string Name, string Key)
        {
            if (string.IsNullOrEmpty(Name))
                return null;
            if (string.IsNullOrEmpty(Key))
                return null;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[Name];
            if ((cookie != null) && (cookie.Values[Key] != null))
            {
                return cookie.Values[Key].ToString();
            }
            else
                return null;
        }
          
        /// <summary>
        /// Set a new value, or update a existing value of a cookie variable
        /// </summary>
        /// <param name="Name">Name of the cookie</param>
        /// <param name="Key">Key of the cookie variable</param>
        /// <param name="value">new value of the cookie key variable</param>
        public static void Set(string Name, string Key, string value)
        {
            if (string.IsNullOrEmpty(Name))
                return;
            if (string.IsNullOrEmpty(Key))
                return;
            if (string.IsNullOrEmpty(value))
                return;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Name];
            if (cookie != null)
            {
                cookie.Values[Key] = value;
            }
            else
            {
                cookie = new HttpCookie(Name);
                cookie.Values.Add(Key, value);
            }

            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
    }

    /// <summary>
    /// All cookie related operation of the project
    /// needs to run through this class.
    /// </summary>
    public static class CookieHelper
    {
        /// <summary>
        /// Check if the cookie value is null
        /// </summary>
        /// <param name="Name">name of the cookie variable</param>
        /// <returns>true if not null or false if null</returns>
        public static bool IsNull(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
                return (HttpContext.Current.Request.Cookies[Name] != null);
            else
                return false;
        }

        /// <summary>
        /// Get the value of the cookie variable
        /// </summary>
        /// <param name="Name">name of the cookie</param>
        /// <param name="Key">key of the cookie variable.
        /// You can have multiple varialbe stored in a one cookie.</param>
        /// <returns></returns>
        public static string Get(string Name, string Key)
        {
            if (string.IsNullOrEmpty(Name))
                return null;
            if (string.IsNullOrEmpty(Key))
                return null;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[Name];
            if ((cookie != null) && (cookie.Values[Key] != null))
            {
                return cookie.Values[Key].ToString();
            }
            else
                return null;
        }

        /// <summary>
        /// Set a new value, or update a existing value of a cookie variable
        /// </summary>
        /// <param name="Name">Name of the cookie</param>
        /// <param name="Key">Key of the cookie variable</param>
        /// <param name="value">new value of the cookie key variable</param>
        public static void Set(string Name, string Key, string value)
        {
            if (string.IsNullOrEmpty(Name))
                return;
            if (string.IsNullOrEmpty(Key))
                return;
            if (string.IsNullOrEmpty(value))
                return;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Name];
            if (cookie != null)
            {
                cookie.Values[Key] = value;
            }
            else
            {
                cookie = new HttpCookie(Name);
                cookie.Values.Add(Key, value);
            }

            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
    }
}