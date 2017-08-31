using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Nido.Common.Utilities.Attributes;
using System.Security.Cryptography;
using Nido.Common.Utilities.MD5;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// The possible Audit Action enumerator
    /// </summary>
    public enum AuditActions
    {
        /// <summary>
        /// Insert
        /// </summary>
        I,
        /// <summary>
        /// Update
        /// </summary>
        U,
        /// <summary>
        /// Delete
        /// </summary>
        D
    }

    /// <summary>
    /// Model uses to store audit trail data in the database
    /// </summary>
    public class AuditTrail : BaseObject, IDisposable
    {
        /// <summary>
        /// primary key
        /// </summary>
        public int AuditTrailId { get; set; }
        /// <summary>
        /// The audited application name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// The time stamp of the operation
        /// </summary>
        public DateTime RevisionStamp { get; set; }
        /// <summary>
        /// The transaction table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Primary key value of the table
        /// </summary>
        public string TablePrimaryId { get; set; }
        /// <summary>
        /// Name of the user, who trigger this transaction 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Whether the action is U|I|D, that is
        /// Update, Insert, Delete
        /// </summary>
        public string Actions { get; set; }
        /// <summary>
        /// The previous dataset that was 
        /// there in the database, before the transaction.
        /// </summary>
        public string OldData { get; set; }
        /// <summary>
        /// The new dataset that is added 
        /// after the transaction
        /// </summary>
        public string NewData { get; set; }
        /// <summary>
        /// Columns that were changed
        /// </summary>
        public string ChangedColumns { get; set; }

        [NotMapped]
        public override bool CanDelete
        {
            get { return true; }
            set { }
        }


        #region IDisposable Members
        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
            _disposed = true;
        }

        #endregion
    }

    /// <summary>
    /// Base object of all the custom objects. 
    /// It is required all the custom objects to inherit from this base object.
    /// </summary>
    /// <example><code  lang="C#">
    /// using System.Collections.Generic;
    /// using System.ComponentModel.DataAnnotations;
    /// using Nido.Common.BackEnd;
    ///    
    /// namespace Nido.Transport.Bll.Models
    /// {
    ///     public class Bank : BaseObject
    ///     {
    ///         public Bank()
    ///         {
    ///         }
    ///   
    ///         // If the primary key is not followed the standard then mark it with the 'Key' Attribute.
    ///         // Standard TableNameId = xyzId
    ///         // As this primary key has followed the standard, 
    ///         // it is not needed to mark it with the 'Key' attribute.
    ///         // We have added this just for the demonstration only.
    ///         [Key]
    ///         public int BankId { get; set; }
    ///         public string Name { get; set; }
    ///         public string Branch { get; set; }
    ///    
    ///         // All the properties that does not map with 
    ///         // the corresponding table fields should have the 'NotMapped' attribute.
    ///         [NotMapped]
    ///         public override string Text
    ///         {
    ///             get
    ///             {
    ///                 return Name;
    ///             }
    ///             set
    ///             {
    ///                 Name = value;
    ///             }
    ///         }
    ///    
    ///    
    ///         // If the foreign key is not followed the standard then mark it with the 'ForeignKey' 
    ///         // Attribute with the name of the related Object.
    ///         // Standard RelatedTableNameId = xyzId
    ///         // As this primary key has followed the standard, 
    ///         // it is not needed to mark it with the 'ForeignKey' attribute.
    ///         // We have added this just for the demonstration only.
    ///         [ForeignKey("HeadOffice")]
    ///         public int HeadOfficeId { get; set; }
    ///         public HeadOffice HeadOffice { get; set; }
    ///         //
    ///         public ICollection&lt;Contractor&gt; Contractors { get; set; }
    ///    
    ///         [NotMapped]
    ///         public override bool CanDelete
    ///         {
    ///             get
    ///             {
    ///                 return true;
    ///             }
    ///             set
    ///             { }
    ///         }
    ///     }
    ///    
    ///     public class HeadOffice
    ///     {
    ///         [Key]
    ///         public int HeadOfficeId { get; set; }
    ///     }
    /// }
    /// </code></example>
    public abstract class BaseObject : Nido.Common.BackEnd.IBaseObject
    {
        /// <summary>
        /// The standard length that uses to cut the display text string
        /// </summary>
        public const int DISPLAY_LENGTH = 15;
        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseObject()
        {
        }

        /// <summary>
        /// User define contructor of the baseobject
        /// </summary>
        /// <param name="displayName">User define name for the display name</param>
        public BaseObject(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Encrypt all properties that marked with the [Encrypt] attribute.
        /// This method encrypt only the properties of the calling model object
        /// </summary>
        /// <typeparam name="T">Pass the type parameter of the Model</typeparam>
        /// <remarks>TODO:The type Parameter does not need to pass. Should be able to remove it</remarks>
        public virtual void EncryptRecords<T>()
            where T : IBaseObject
        {
            PropertyInfo[] allEntityProperties = this.GetType().GetProperties();
            foreach (PropertyInfo prop in allEntityProperties)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    EncryptedAttribute authAttr = attr as EncryptedAttribute;
                    if (authAttr != null)
                    {
                        object propName = prop.GetValue(this, null);
                        bool doEncrypt = authAttr.DoEncrypt;
                        if (doEncrypt)
                        {
                            if (prop.PropertyType == typeof(string))
                                prop.SetValue(this, Crypto.Encrypt(propName.ToString()), null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dencrypt all properties that marked with the [Encrypt] attribute.
        /// This method dencrypt only the properties of the calling model object
        /// </summary>
        /// <typeparam name="T">Pass the type parameter of the Model</typeparam>
        /// <remarks>TODO:The type Parameter does not need to pass. Should be able to remove it</remarks>
        public virtual void DecryptRecords<T>()
            where T : IBaseObject
        {
            PropertyInfo[] allEntityProperties = this.GetType().GetProperties();
            foreach (PropertyInfo prop in allEntityProperties)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    EncryptedAttribute authAttr = attr as EncryptedAttribute;
                    if (authAttr != null)
                    {
                        object propName = prop.GetValue(this, null);
                        bool doEncrypt = authAttr.DoEncrypt;
                        if (doEncrypt)
                        {
                            if (prop.PropertyType == typeof(string))
                                prop.SetValue(this, Crypto.Decrypt(propName.ToString()), null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a special popup enabled text to be displayed this column in a table or a gridview.
        /// Telerik: This need HtmlEncode='false'
        /// HTML Table: To be verified
        /// </summary>
        /// <param name="text">Text to be displayed</param>
        /// <returns>specially created string</returns>
        /// <remarks>This requires UI to understand the special style named 'info'</remarks>
        public virtual string CreatePopupText(string text)
        {
            if ((text != null) && (text.Length > DISPLAY_LENGTH))
            {
                string tempStr = "";
                tempStr = text.Substring(0, DISPLAY_LENGTH);
                return "<a href='#' class='info'>" + tempStr + "...<span>" + text + "</span>";
            }
            else return text;
        }

        /// <summary>
        /// Create a special popup enabled text to be displayed this column in a table or a gridview.
        /// This needs to set HtmlEncode='false' and a.info style in place (check BaseObject source to find the style).
        /// </summary>
        /// <param name="title">Text to be displayed. Any html tags are supported for the title</param>
        /// <param name="text">Text to be displayed in popup. Any html tags are supported for the Text</param>
        /// <returns>specially created string</returns>
        /// <remarks>This requires UI to understand the special style named 'info'
        /// a.info
        /// {
        ///     text-decoration: none;
        ///     position: relative; /*this is the key*/
        ///     
        ///     color: #000;
        ///     border-bottom-style: dotted;
        ///     border-bottom-width: thin;
        ///     margin-right: 10px;
        /// }
        /// 
        /// a.info:hover
        /// {
        ///     z-index: 25;
        ///     background-color: #ff0;
        /// }
        /// 
        /// a.info span
        /// {
        ///     display: none;
        /// }
        /// 
        /// a.info:hover span
        /// {
        ///     /*the span will display just on :hover state*/
        ///     display: block;
        ///     position: absolute;
        ///     top: 2em;
        ///     left: 2em;
        ///     border: 1px solid #0cf;
        ///     background: rgb(252, 252, 252);
        ///     color: #000;
        ///     text-align: left;
        ///     z-index: 525;
        /// }
        /// 
        /// </remarks>
        public virtual string CreatePopupText(string title, string text)
        {
            if ((text != null) && (text.Length > title.Length))
            {
                //string tempStr = "";
                //tempStr = text.Substring(0, DISPLAY_LENGTH);
                return "<a href='#' class='info'>" + title + "...<span>" + text + "</span>";
            }
            else return text;
        }

        /// <summary>
        /// Display name of the object
        /// </summary>
        [NotMapped]
        [ScaffoldColumn(false)]
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// Implement this to prevent or allow the object to be deleted by the base handler.
        /// You need to consider all dependant objects before allowing a object to be deleted.
        /// This will happen automatically.
        /// </summary>
        /// <example><code lang="C#">
        /// Sample implementation of the CanDelete Property is given below
        /// [NotMapped]
        /// public override bool CanDelete
        /// {
        ///    get
        ///    {
        ///         return ((Contractors != null) AND (Contractors.Count &gt; 0));
        ///    }
        ///    set
        ///    { }
        /// }
        /// </code></example>
        public abstract bool CanDelete { get; set; }
        /// <summary>
        /// This will be used to represent the Display property of this record.
        /// You have to override this if you are 
        /// to use the BaseCombo object to populate dropdown box with this value.
        /// </summary>
        /// <example><code lang="C#">
        /// [NotMapped]
        /// public override string Text
        /// {
        ///     get
        ///     {
        ///          return Name;
        ///     }
        ///     set
        ///     {
        ///          Name = value;
        ///     }
        /// }
        /// </code></example>
        [NotMapped]
        [ScaffoldColumn(false)]
        public virtual string Text { get { return "Value not Set!!"; } set { } }
        /// <summary>
        /// This will be used to represent the value property of this record.
        /// You have to override this if you are 
        /// to use the BaseCombo object to populate dropdown box with this value.
        /// </summary>
        /// <example><code lang="C#">
        /// [NotMapped]
        /// public override string Value
        /// {
        ///     get
        ///     {
        ///          return BankId.ToString();
        ///     }
        ///     set
        ///     {
        ///          base.value = value;
        ///     }
        /// }
        /// </code></example>
        [NotMapped]
        [ScaffoldColumn(false)]
        public virtual string Value { get; set; }
        /// <summary>
        /// This field is used to represent the name of the respective text field. The default is "Text". 
        /// You can always assign custom values for this field.
        /// </summary>
        [NotMapped]
        [ScaffoldColumn(false)]
        public static string DataTextField
        {
            get
            {
                return (string.IsNullOrEmpty(_DataTextField))
                    ? "Text" : _DataTextField;
            }
            set { _DataTextField = value; }
        }
        private static string _DataTextField;
        /// <summary>
        /// This field is used to represent the value of the respective value field. The default is "Value". 
        /// You can always assign custom values for this field.
        /// </summary>
        [NotMapped]
        [ScaffoldColumn(false)]
        public static string DataValueField { get; set; }
        /// <summary>
        /// The Delete error message, which is to be displayed to the used can be set here.
        /// You may override this property to set your custom error messages.
        /// </summary>
        [NotMapped]
        [ScaffoldColumn(false)]
        public virtual string DeleteError
        {
            get
            {
                return (!CanDelete) ?
                    "This record cannot be deleted as that has some dependacies"
                    : "";
            }
        }
        /// <summary>
        /// This gives a pure text version of the delete dependancy infomation
        /// </summary>
        [NotMapped]
        [ScaffoldColumn(false)]
        public virtual string DeleteErrorText
        {
            get
            {
                return (!CanDelete) ?
                    "This record cannot be deleted as that has some dependacies"
                    : "";
            }
        }

        /// <summary>
        /// The class creates a html table (display its records as a popup message)
        /// using the given entity
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <example>
        /// <code>
        /// [NotMapped]
        /// public string HomeAddressPopup
        /// {
        ///     get
        ///      {
        ///         if (this.HomeAddress != null)
        ///             return this.CreatePopupText(this.HomeAddress.Text
        ///                 , new TableCreator<Address>(this.HomeAddress)
        ///                 .RemoveRecord(x => x.Value).RemoveRecord(x => x.Text)
        ///                 .RemoveRecord(x => x.DeleteError).RemoveRecord(x => x.CanDelete)
        ///                 .GetPopupTable());
        ///         else
        ///             return "Indecisive record data!!";
        ///     }
        /// }
        /// </code>
        /// </example>
        protected class TableCreator<E>
            where E : BaseObject
        {
            private List<string> listToRemove = new List<string>();
            private List<string> listOnlyUse = new List<string>();
            private E _Entity;
            /// <summary>
            /// User define constructor of the TableCreator class
            /// </summary>
            /// <param name="entity">The entity type to be used to create the table</param>
            public TableCreator(E entity)
            {
                _Entity = entity;
            }

            /// <summary>
            /// Remove any records from the table before creating the table.
            /// </summary>
            /// <param name="path">Record to be removed</param>
            /// <returns>The current TableCreator object instance</returns>
            /// <example>
            /// <code lang="C#">
            /// new TableCreator&lt;Address>(this.HomeAddress)
            ///            .RemoveRecord(x => x.Value).RemoveRecord(x => x.Text)
            ///            .RemoveRecord(x => x.DeleteError).RemoveRecord(x => x.CanDelete)
            ///            .GetPopupTable()
            /// </code>
            /// </example>
            public TableCreator<E> RemoveRecord(Expression<Func<E, object>> path)
            {
                if (path != null)
                {
                    MemberExpression body = path.Body as MemberExpression;

                    if (body == null)
                    {
                        UnaryExpression ubody = (UnaryExpression)path.Body;
                        body = ubody.Operand as MemberExpression;
                    }
                    listToRemove.Add(body.Member.Name);
                }
                return this;
            }

            /// <summary>
            /// Add only the given entity records to the popup table. The RemoveRecord values will be discarded..
            /// </summary>
            /// <param name="path">Record to be removed</param>
            /// <returns>The current TableCreator object instance</returns>
            /// <example>
            /// <code lang="C#">
            /// new TableCreator&lt;Address>(this.HomeAddress)
            ///            .OnlyUseRecord(x => x.Value).OnlyUseRecord(x => x.Text)
            ///            .OnlyUseRecord(x => x.DeleteError).OnlyUseRecord(x => x.CanDelete)
            ///            .GetPopupTable()
            /// </code>
            /// </example>
            public TableCreator<E> OnlyUseRecord(Expression<Func<E, object>> path)
            {
                if (path != null)
                {
                    MemberExpression body = path.Body as MemberExpression;

                    if (body == null)
                    {
                        UnaryExpression ubody = (UnaryExpression)path.Body;
                        body = ubody.Operand as MemberExpression;
                    }
                    listOnlyUse.Add(body.Member.Name);
                }
                return this;
            }

            /// <summary>
            /// Create the HTML table that is to be used as the popup text.
            /// This method should call only after removing records 
            /// that are not needed to include in the table.
            /// </summary>
            /// <returns>String version of the created table</returns>
            /// <example>
            /// <code lang="C#">
            /// new TableCreator&lt;Address>(this.HomeAddress)
            ///            .RemoveRecord(x => x.Value).RemoveRecord(x => x.Text)
            ///            .RemoveRecord(x => x.DeleteError).RemoveRecord(x => x.CanDelete)
            ///            .GetPopupTable()
            /// </code>
            /// </example>
            public string GetPopupTable()
            {
                string r = "";
                if (_Entity != null)
                {
                    PropertyInfo[] allEntityProperties = _Entity.GetType().GetProperties();
                    r = @"<div style='border: 1px solid rgb(241, 240, 240);'>
                        <div style='border: 1px solid rgb(240, 240, 241);'>
                        <table style='width: 450px; border: 1px solid rgb(196, 214, 228); padding: 4px;'>";
                    foreach (PropertyInfo propertyInfo in allEntityProperties)
                    {
                        if ((listOnlyUse.Count > 0) && (listOnlyUse.Contains(propertyInfo.Name)))
                        {
                            r += "<tr><td style='font-size: 11px; color: rgb(89, 77, 90);'><b>" + propertyInfo.Name + "<b><td>:</td></td><td>"
                                + propertyInfo.GetValue(_Entity, null) + "</td></tr>";
                        }
                        else if ((listOnlyUse.Count == 0) && (!listToRemove.Contains(propertyInfo.Name)))
                            r += "<tr><td style='font-size: 11px; color: rgb(89, 77, 90);'><b>" + propertyInfo.Name + "<b><td>:</td></td><td>"
                                + propertyInfo.GetValue(_Entity, null) + "</td></tr>";
                    }
                    r += "</table></div><div>";
                }
                listOnlyUse.Clear();
                listToRemove.Clear();
                return r;
            }
        }
    }
}
