using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Nido.Common
{
    /// <summary>
    /// The session related data store, 
    /// retrieve needed to be done through this class
    /// </summary>
    public static class SessionHelper
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
        public static void SetValue(string key, object value)
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
        public static T GetValue<T>(string key, CreateItem<T> creator)
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
        public static T GetValue<T>(string key)
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