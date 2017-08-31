using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace Nido.Common
{
	/// <summary>
	/// This Class is used to read configuration settings from the app.config file.
	/// </summary>
	public sealed class ConfigSettings 
	{
		#region Constants

		/// <summary>
		/// Database connection string.
		/// </summary>
		public const string DB_CONNECTION_STRING = "DBConnectionString";

		/// <summary>
		/// Key uses to fetch the Database Provider. "SQL, Oracle, etc..."
		/// </summary>
		public const string DB_PROVIDER = "DBProvider";
        /// <summary>
        /// This configuration property will enable tracking all DB transaction
        /// </summary>
        public const string ENABLE_AUDIT_TRAIL = "EnableAuditTrail";
        public const string ENABLE_TESTING = "EnableTesting";
        public const string SYSTEM_NAME = "SystemName";
        public const string ENABLE_OPT_TRACKING = "EnableOptTracking";

		#endregion

		#region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
		public ConfigSettings()
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads the given configuration value from the app.config. 
		/// If the key does not exists then returns the default value.
		/// </summary>
		/// <param name="key">Configuration key to be read.</param>
		/// <param name="defaultValue">Default value of the key.</param>
		/// <returns></returns>
        public static string ReadConfigValue(string key, string defaultValue)
        {
            string configValue;

            try
            {
                configValue = ConfigurationManager.AppSettings[key];
                if (configValue == null)
                    configValue = defaultValue;
            }
            catch// (Exception ex)
            {
                configValue = defaultValue;
            }

            return configValue;
        }

		/// <summary>
		/// Reads the value of the given configuration key.
		/// </summary>
		/// <param name="key">Configuration Key to be read.</param>
		/// <returns></returns>
		public static string ReadConfigValue(string key)
		{
			return ReadConfigValue(key, string.Empty);
		}

		#endregion
	}
}
