using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using Nido.Common;
using System.Data.Common;
using System.Data.Entity;
using EntityFramework.Audit;
using Nido.Common.Utilities.Attributes;
using System.Collections;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// BaseObjectContext defines the default implementation for any DB Context created.
    /// All the context class created need to derive from this base class.
    /// 
    /// </summary>
    public abstract class BaseObjectConext : DbContext, IDisposable, IBaseObjectConext
    {
        /// <summary>
        /// Indicate whether tracking is enabled for this system.
        /// The tracking insert transaction data in to the AuditTrail table 
        /// </summary>
        public bool EnableOptTracking { get; set; }

        /// <summary>
        /// Indicate whether the system is enabled for testing.
        /// Once the testing is enabled the system will not modify the 
        /// actual database, but keep a virtual form of the database 
        /// allowing user to test the system.
        /// </summary>
        public bool EnableTesting { get; set; }

        public bool EnableAuditTrail { get; set; }
        /// <summary>
        /// Default Contructor of the BaseObjectContext class
        /// 
        ///	I add an example for the InversePropertyAttribute. 
        ///	It cannot only be used for relationships in self referencing 
        ///	entities (as in the example linked in Ladislav's answer) 
        ///	but also in the "normal" case of relationships between different entities:
        ///	</summary>
        /// <example>
        /// <code lang="C#">
        ///	public class Book
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Title { get; set; }
        ///	
        ///	    [InverseProperty("Books")]
        ///	    public Author Author { get; set; }
        ///	}
        ///	
        ///	public class Author
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Name { get; set; }
        ///	
        ///	    [InverseProperty("Author")]
        ///	    public virtual ICollection&lt;Book> Books { get; set; }
        ///	}
        ///	
        ///	This would describe the same relationship as this Fluent Code:
        ///	
        ///	modelBuilder.Entity&lt;Book>()
        ///	            .HasOptional(b => b.Author)
        ///	            .WithMany(a => a.Books);
        ///	
        ///	... or ...
        ///	
        ///	modelBuilder.Entity&lt;Author>()
        ///	            .HasMany(a => a.Books)
        ///	            .WithOptional(b => b.Author);
        ///	
        ///	Now, adding the InverseProperty attribute in the example above is redundant: 
        ///	The mapping conventions would create the same single relationship anyway.
        ///	
        ///	But consider this example (of a book library which only contains books written together by two authors):
        ///	
        ///	public class Book
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Title { get; set; }
        ///	
        ///	    public Author FirstAuthor { get; set; }
        ///	    public Author SecondAuthor { get; set; }
        ///	}
        ///	
        ///	public class Author
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Name { get; set; }
        ///	
        ///	    public virtual ICollection&lt;Book> BooksAsFirstAuthor { get; set; }
        ///	    public virtual ICollection&lt;Book> BooksAsSecondAuthor { get; set; }
        ///	}
        ///	
        ///	The mapping conventions would not detect which ends of these relationships belong 
        ///	together and actually create four relationships (with four foreign keys in the Books table). 
        ///	In this situation using theInverseProperty would help to define the correct relationships we want in our model:
        ///	
        ///	public class Book
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Title { get; set; }
        ///	
        ///	    [InverseProperty("BooksAsFirstAuthor")]
        ///	    public Author FirstAuthor { get; set; }
        ///	    [InverseProperty("BooksAsSecondAuthor")]
        ///	    public Author SecondAuthor { get; set; }
        ///	}
        ///	
        ///	public class Author
        ///	{
        ///	    public int ID { get; set; }
        ///	    public string Name { get; set; }
        ///	
        ///	    [InverseProperty("FirstAuthor")]
        ///	    public virtual ICollection&lt;Book> BooksAsFirstAuthor { get; set; }
        ///	    [InverseProperty("SecondAuthor")]
        ///	    public virtual ICollection&lt;Book> BooksAsSecondAuthor { get; set; }
        ///	}
        ///	
        ///	Here we would only get two relationships. 
        /// (Note: The InverseProperty attribute is only necessary on one end of the relationship, 
        /// we can omit the attribute on the other end.)
        /// </code></example>
        public BaseObjectConext()
            : base()
        {
            EnableTesting = Convert.ToBoolean(ConfigSettings.ReadConfigValue(ConfigSettings.ENABLE_TESTING, "false"));
            EnableOptTracking = Convert.ToBoolean(ConfigSettings.ReadConfigValue(ConfigSettings.ENABLE_OPT_TRACKING, "false"));
            EnableAuditTrail = Convert.ToBoolean(ConfigSettings.ReadConfigValue(ConfigSettings.ENABLE_AUDIT_TRAIL, "false"));

            if (EnableAuditTrail)
            {
                if (SessionHelper.SessionState != null)
                {
                    UserName = SessionHelper.GetValue<string>("UserName", delegate() { return "Not Set"; });
                    SystemName = SessionHelper.GetValue<string>("SystemName", delegate() { return "Not Set"; });
                }

                this.CurrentObjectContext.SavingChanges += new EventHandler(CurrentObjectContext_SavingChanges);
            }
        }

        /// <summary>
        /// User define constractor that accept the connection string as a parameter
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseObjectConext(String connectionString)
            : base(connectionString)
        {
            // set default connection string
            this.Database.Connection.ConnectionString = connectionString;
        }

        public BaseObjectConext(DbCompiledModel model)
            : base(model)
        {
        }

        public BaseObjectConext(DbConnection existingConnection, bool contextOwnConnection)
            : base(existingConnection, contextOwnConnection)
        {
        }

        public BaseObjectConext(string connectionString, DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public BaseObjectConext(System.Data.Entity.Core.Objects.ObjectContext objectContext, bool dbContextOwnConnection)
            : base(objectContext, dbContextOwnConnection)
        {
        }

        public BaseObjectConext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnConnection)
            : base(existingConnection, model, contextOwnConnection)
        {
        }

        /// <summary>
        /// Use this method to call to a stored procedure or a T-SQL command.
        /// </summary>
        /// <typeparam name="T">Type of the returning object. 
        /// This can directly be mapped to  flat business object.</typeparam>
        /// <param name="nameOfSp">Name of the stored procedure</param>
        /// <returns>List of object returned by the stored procedure</returns>
        /// <example>
        /// <code lang="C#">
        /// public IEnumerable&lt;PRHeader_S_FinancePRStatus> GetPRForFinance()
        /// {
        ///    try
        ///    {
        ///        return db.ExecuteStoredProcedure&lt;PRHeader_S_FinancePRStatus>("PRHeader_S_FinancePRStatus");
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        throw ex;
        ///    }
        /// }
        /// 
        /// </code>
        /// </example>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string nameOfSp)
        {
            IEnumerable<T> entityList = ((IObjectContextAdapter)this)
                .ObjectContext.ExecuteStoreQuery<T>(nameOfSp);
            return entityList;
        }

        /// <summary>
        /// Use this method to call to a stored procedure or a T-SQL command with a given set of parameters
        /// </summary>
        /// <typeparam name="T">Type of the returning object. 
        /// This can directly be mapped to  flat business object.</typeparam>
        /// <param name="nameOfSp">Name of the stored procedure</param>
        /// <param name="parameters">list of paramters to be passed on the SP.</param>
        /// <returns>List of object returned by the stored procedure</returns>
        /// <example>
        /// <code lang="C#">
        /// 
        /// public IEnumerable&lt;PRHeader_S_FinancePRStatus> GetPRForFinance(int prStatusId)
        /// {
        ///    try
        ///    {
        ///        return db.ExecuteStoredProcedure&lt;PRHeader_S_FinancePRStatus>
        ///        ("PRHeader_S_FinancePRStatus @BudgetStatusId,@PRStatusId",
        ///             new SqlParameter("@BudgetStatusId", 1), new SqlParameter("@PRStatusId", prStatusId));
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        throw ex;
        ///    }
        /// }
        /// 
        /// </code>
        /// </example>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string nameOfSp
            , params object[] parameters)
        {
            IEnumerable<T> entityList = ((IObjectContextAdapter)this)
                .ObjectContext.ExecuteStoreQuery<T>(nameOfSp, parameters);

            return entityList;
        }

        /// <summary>
        /// Use this method to call to a stored procedure or a T-SQL command with a given set of parameters.
        /// This does not return a list of object but just an integer indicating the status of the call.
        /// </summary>
        /// <param name="nameOfSp">Name of the stored procedure</param>
        /// <param name="parameters">list of paramters to be passed on the SP.</param>
        /// <returns>integer value indicating the status of the SP call</returns>
        /// <example>
        /// <code lang="C#">
        /// 
        /// public int GetPRForFinance(int prStatusId)
        /// {
        ///    try
        ///    {
        ///        return db.ExecuteStoredProcedure("PRHeader_S_FinancePRStatus @BudgetStatusId,@PRStatusId",
        ///             new SqlParameter("@PRStatusId", prStatusId));
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        throw ex;
        ///    }
        /// }
        /// 
        /// </code>
        /// </example>
        public int ExecuteStoredProcedure(string nameOfSp
            , params object[] parameters)
        {
            return ((IObjectContextAdapter)this)
                .ObjectContext.ExecuteStoreCommand(nameOfSp, parameters);
        }

        /// <summary>
        /// This removes the prural sign from the table object. 
        /// This way it is allowed to use the plural sign for list object of the respective table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// The name of the user. To be used for tracking transactions
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The name of the System. To be used for tracking transactions
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// DB table representation of the AuditTrail table
        /// </summary>
        public DbSet<AuditTrail> AuditTrails { get; set; }

        /// <summary>
        /// The ObjectContext Representation of the current DbContext object
        /// </summary>
        public System.Data.Entity.Core.Objects.ObjectContext CurrentObjectContext
        {
            get
            {
                return ((IObjectContextAdapter)this)
                    .ObjectContext;
            }
        }

        /// <summary>
        /// The event that triggers when 
        /// the context savechanges method is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentObjectContext_SavingChanges(object sender, EventArgs e)
        {
            try
            {
                ChangeTracker.DetectChanges(); // Important!
                System.Data.Entity.Core.Objects.ObjectContext ctx = ((IObjectContextAdapter)this).ObjectContext;
                List<System.Data.Entity.Core.Objects.ObjectStateEntry> objectStateEntryList =
                    ctx.ObjectStateManager.GetObjectStateEntries(System.Data.Entity.EntityState.Added
                                                               | System.Data.Entity.EntityState.Modified
                                                               | System.Data.Entity.EntityState.Deleted)
                    .ToList();

                foreach (System.Data.Entity.Core.Objects.ObjectStateEntry entry in objectStateEntryList)
                {
                    object[] list = entry.Entity.GetType().GetCustomAttributes(false);
                    // Only the models that marked with 'Auditable' attribute will be tracked
                    // Inside the model the properties that need to tracked needed to be marked with
                    // Auditable attribute
                    if (list.Count() == 0 || !((Nido.Common.Utilities.Attributes.AuditableAttribute)(list[0])).DoAudit)
                        continue;
                   
                    TypeAttributes te = entry.Entity.GetType().Attributes;
                    AuditTrail audit = new AuditTrail();
                    audit.RevisionStamp = DateTime.Now;
                    audit.TableName = entry.EntitySet.Name + entry.EntityKey;
                    audit.UserName = UserName;
                    audit.SystemName = SystemName;
                    if (!entry.IsRelationship)
                    {
                        switch (entry.State)
                        {
                            case System.Data.Entity.EntityState.Added:
                                // write log...
                                {
                                    audit.NewData = GetEntryValueInString(entry);
                                    audit.Actions = AuditActions.I.ToString();
                                }
                                break;
                            case System.Data.Entity.EntityState.Deleted:
                                // write log...
                                {
                                    audit.TablePrimaryId = GetKeyValue(entry);
                                    audit.OldData = GetEntryValueInString(entry);
                                    audit.Actions = AuditActions.D.ToString();
                                }
                                break;
                            case System.Data.Entity.EntityState.Modified:
                                {
                                    string xmlOld = "<?xml version='1.0' encoding='utf-16'?>  <" + (entry.EntitySet).Name
                                        + " xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";
                                    string xmlNew = "<?xml version='1.0' encoding='utf-16'?>  <" + (entry.EntitySet).Name
                                        + " xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";

                                    PropertyInfo[] propList = entry.Entity.GetType().GetProperties();
                                    Dictionary<string, object> attrList = new Dictionary<string, object>();
                                    foreach (PropertyInfo pInfo in propList)
                                    {
                                        object[] atts = pInfo.GetCustomAttributes(typeof(AuditableAttribute), false);
                                        if (atts.Length > 0)
                                            attrList.Add(pInfo.Name, atts[0]);
                                    }
                                    int i = 0;
                                    object[] listte = entry.GetUpdatableOriginalValues()[0].GetType().GetCustomAttributes(false);
                                    foreach (string propertyName in
                                                 entry.GetModifiedProperties())
                                    {
                                        if (attrList.Keys.Contains(propertyName))
                                        {
                                            DbDataRecord original = entry.OriginalValues;

                                            string oldValue = original.GetValue(
                                                original.GetOrdinal(propertyName))
                                                .ToString();

                                            System.Data.Entity.Core.Objects.CurrentValueRecord current = entry.CurrentValues;
                                            string newValue = current.GetValue(
                                                current.GetOrdinal(propertyName))
                                                .ToString();

                                            xmlOld += "<" + propertyName + " type='" + original.GetFieldType(i) + "'>" + oldValue + "</" + propertyName + ">";
                                            xmlNew += "<" + propertyName + " type='" + original.GetFieldType(i) + "'>" + newValue + "</" + propertyName + ">";
                                        }
                                        i++;
                                    }

                                    xmlOld += "</" + (entry.EntitySet).Name + ">";
                                    xmlNew += "</" + (entry.EntitySet).Name + ">";

                                    audit.OldData = xmlOld;
                                    audit.NewData = xmlNew;

                                    audit.TablePrimaryId = GetKeyValue(entry);
                                    audit.Actions = AuditActions.U.ToString();
                                    break;
                                }

                        }
                    }
                    AuditTrails.Add(audit);
                }
            }
            catch
            {
                // Keep quite...
            }
        }

        protected override System.Data.Entity.Validation.DbEntityValidationResult 
            ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            return base.ValidateEntity(entityEntry, items);
        }

        /// <summary>
        /// Get the records of the object that will 
        /// be inserted to the table in the form of a XML file
        /// </summary>
        /// <param name="entry">ObjectStateEntry that carries the 
        /// original and current values of the table</param>
        /// <returns>XML string of the respective table record</returns>
        private string GetEntryValueInString(System.Data.Entity.Core.Objects.ObjectStateEntry entry)
        {
            try
            {
                string xml = "<?xml version='1.0' encoding='utf-16'?>  <" 
                    + (entry.EntitySet).Name 
                    + " xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";
                var currentValues = (entry.State == System.Data.Entity.EntityState.Deleted) ? entry.OriginalValues : entry.CurrentValues;
                for (int i = 0; i < currentValues.FieldCount; i++)
                {

                    //<StudentName type="int">Nirosh</StudentName
                    xml += "<" + currentValues.GetName(i) + " type='" 
                        + currentValues.GetFieldType(i) + "'>" 
                        + currentValues.GetValue(i) + "</" 
                        + currentValues.GetName(i) + ">";
                }
                xml += "</" + (entry.EntitySet).Name + ">";
                return xml;
            }
            catch (Exception e)
            {
                return "<?xml version='1.0' encoding='utf-16'?>  <Error xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>" + e.Message + "</Error>";
            }
        }

        public string GetKeyValue(ObjectStateEntry entry)
        {
            var objectStateEntry = this.CurrentObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                return objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();

            return "0";
        }

        /// <summary>
        /// The override enable stop saving changes to the database 
        /// when the system is in test enable mode.
        /// </summary>
        /// <returns>status of the save</returns>
        public override int SaveChanges()
        {
            if (EnableTesting)
                return 1;
            else
                return base.SaveChanges();
        }

        private bool _disposed;

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual new void Dispose(bool disposing)
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

    }
}
