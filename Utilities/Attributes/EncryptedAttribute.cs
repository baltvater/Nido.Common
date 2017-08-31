using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Nido.Common.Utilities.Attributes
{
    /// <summary>
    /// Mark the class to make it auditable 
    /// so then it will create new entries to the AuditTrail table
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false)]
    public sealed class EncryptedAttribute : Attribute
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public EncryptedAttribute() { }

        private bool _doEncrypt = true;
        /// <summary>
        /// Set this to false if you do not want to 
        /// audit a property of a class that set to audit
        /// </summary>
        public bool DoEncrypt
        {
            get { return _doEncrypt; }
            set { _doEncrypt = value; }
        }

        /// <summary>
        /// User defined constructor that take 
        /// the doAudit boolean as an input
        /// </summary>
        /// <param name="doAudit"></param>
        public EncryptedAttribute(bool doEncrypt)
        {
            _doEncrypt = doEncrypt;
        }
    }
}
