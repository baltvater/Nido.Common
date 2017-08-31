using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Caching;
using System.Web.SessionState;
using System.Web;
using System.Web.UI;

namespace Nido.Common.Utilities.Cache
{
    /// <summary>
    /// This gives a reference to CacheManager
    /// </summary>
    /// <example><code lang="C#">
    ///  StudentHandler handlerStudent = new StudentHandler();
    ///  // cache assigned Students
    ///
    ///  var tasks = handlerStudent
    ///      .GetAllFromCache(x => x.IsActive == true, 
    ///      new NidoCachePolicy(300), 
    ///      tags: new[] { "ActStudents", "All sActive Students" });
    ///
    ///  // some update happened to Students, so expire ActStudents tag
    ///  NidoCacheManager.Current.Expire("ActStudents");
    /// </code>
    /// </example>
    public class NidoCacheManager : CacheManager
    {
        /// <summary>
        /// Expire only the current cache policy
        /// </summary>
        /// <param name="tag"></param>
        public void ExpireMe(string tag)
        {
            Current.Expire(tag);
        }

        /// <summary>
        /// All cookie related operation of the project
        /// needs to run through this class.
        /// </summary>
        public static class Cookie
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
            public static string Retrieve(string Name, string Key)
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
            public static void Store(string Name, string Key, string value)
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
        /// The session related data store, 
        /// retrieve needed to be done through this class
        /// </summary>
        public static class Session
        {
            /// <summary>
            /// Get the state of the session
            /// </summary>
            public static HttpSessionState SessionState
            {
                get
                {
                    if (HttpContext.Current == null) return null;
                    return HttpContext.Current.Session;
                }
            }
            /// <summary>
            /// A delegate used by <see cref="GetValue{T}(string)"/> to retrieve the default, or initial,
            /// value of an object to be retrieved from the state.
            /// </summary>
            /// <typeparam name="T">The object type</typeparam>
            /// <returns>The default/initial instance of the object.</returns>
            public delegate T CreateItem<T>();

            /// <summary>
            /// Sets the value in the session.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public static void Store(string key, object value)
            {
                HttpSessionState state = SessionState;
                if (state == null) return;
                state[key] = value;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <typeparam name="T">Type of object to return</typeparam>
            /// <param name="key">The key.</param>
            /// <param name="creator">A method used to create the default, or initial value, if not found in the session.</param>
            /// <returns>The object, if found, or the return value of <paramref name="creator"/> if not.</returns>
            public static T Retrieve<T>(string key, CreateItem<T> creator)
            {
                HttpSessionState state = SessionState;
                if (state == null) return default(T);

                object result = state[key];

                if (result == null)
                {
                    result = creator();
                    state[key] = result;
                }

                return (T)result;
            }

            /// <summary>
            /// Get a value of a store session object
            /// </summary>
            /// <typeparam name="T">Respective return type of the object. 
            /// You need to give the correct type of the returning object here.</typeparam>
            /// <param name="key">The Key that uses to store this value</param>
            /// <returns></returns>
            public static T Retrieve<T>(string key)
            {
                HttpSessionState state = SessionState;
                if (state == null) return default(T);

                object result = state[key];

                if (result == null)
                {
                    result = default(T);
                    state[key] = result;
                }

                return (T)result;
            }
        }
    }
}
