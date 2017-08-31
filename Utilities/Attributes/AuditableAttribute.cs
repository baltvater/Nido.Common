using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;

namespace Nido.Common.Utilities.Attributes
{
    /// <summary>
    /// Mark the class to make it auditable 
    /// so then it will create new entries to the AuditTrail table
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false)]
    public sealed class AuditableAttribute : Attribute
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public AuditableAttribute() { }

        private bool _doAudit = true;
        /// <summary>
        /// Set this to false if you do not want to 
        /// audit a property of a class that set to audit
        /// </summary>
        public bool DoAudit
        {
            get { return _doAudit; }
            set { _doAudit = value; }
        }

        /// <summary>
        /// User defined constructor that take 
        /// the doAudit boolean as an input
        /// </summary>
        /// <param name="doAudit"></param>
        public AuditableAttribute(bool doAudit) 
        {
            _doAudit = doAudit;
        }
    }
}
