using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nido.Common.Exceptions;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Reflection;
using Nido.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using EntityFramework.Caching;
using EntityFramework.Extensions;
using System.Linq;
using Nido.Common.Utilities.Cache;
using Nido.Common.Utilities.Attributes;
using System.Data.Entity.Validation;
using RefactorThis.GraphDiff;
using KellermanSoftware.CompareNetObjects;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// Implement all the handlers of the business logic layer with this class.
    /// </summary>
    /// <typeparam name="E">Custom business object, which is created after deriving from the </typeparam>
    /// <typeparam name="C">Database Context class that created by inheritting BaseObjectConext class</typeparam>
    /// <example><code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using MasIt.Common.BackEnd;
    /// using MasIt.Transport.Bll.Models;
    /// using MasIt.Transport.Bll.DB;
    /// 
    /// namespace MasIt.Transport.Bll.Handlers
    /// {
    ///     public class DriverHandler : HandlerBase{Driver, TransportDBContext}
    ///     {
    ///         protected override Type LogPrefix
    ///         {
    ///             get { return this.GetType(); }
    ///         }
    ///     }
    /// }
    /// 
    /// // This is a complete solution implemented using the Common Framework
    /// 
    ///	using System;
    ///	using System.Collections.Generic;
    ///	using System.Linq;
    ///	using System.Web;
    ///	using System.Web.UI;
    ///	using System.Web.UI.WebControls;
    ///	using MasDirectBrand.BLL.Handlers;
    ///	using MasDirectBrand.BLL.Models;
    ///	using MasIt.Common.BackEnd;
    ///	using MasIt.Common.Web;
    ///	using MasIt.Common;
    ///	using System.Text;
    ///	using MasIt.Common.FrontEnd;
    ///	using System.Linq.Expressions;
    ///	using MasIt.Common.Utilities.Alerting.Sms;
    ///	using MasIt.Common.Utilities.Alerting.Email;
    ///	
    ///	namespace DemoTestApplication.UserControls
    ///	{
    ///	    public partial class ucStudentList : BaseUserControl&lt;IEnumerable&lt;Student>>
    ///	    {
    ///	        public const int PAGE_SIZE = 10;
    ///	
    ///	        public enum ControlModes
    ///	        {
    ///	            NavDisabled,
    ///	            NavEnabled,
    ///	            NextDisabled,
    ///	            PreviousDisabled
    ///	        }
    ///	
    ///	        public int CurrentPage
    ///	        {
    ///	            get
    ///	            {
    ///	                return ViewStateHelper.GetValue&lt;int>(this.ViewState, "CurrentPage", 0);
    ///	            }
    ///	            set
    ///	            {
    ///	                ViewStateHelper.SetValue(this.ViewState, "CurrentPage", value);
    ///	            }
    ///	        }
    ///	
    ///	        private int _TotalItemCount;
    ///	        public int TotalItemCount
    ///	        {
    ///	            get
    ///	            {
    ///	                return _TotalItemCount;
    ///	            }
    ///	            set
    ///	            {
    ///	                _TotalItemCount = value;
    ///	                if (value &lt; PAGE_SIZE)
    ///	                    this.ResetUserInterface(ControlModes.NavDisabled);
    ///	                else if (CurrentPage == 0)
    ///	                    this.ResetUserInterface(ControlModes.PreviousDisabled);
    ///	                else if (CurrentPage * PAGE_SIZE + PAGE_SIZE > TotalItemCount)
    ///	                    this.ResetUserInterface(ControlModes.NextDisabled);
    ///	                else
    ///	                    this.ResetUserInterface(ControlModes.NavEnabled);
    ///	            }
    ///	        }
    ///	
    ///	        public string PageNumberPanel
    ///	        {
    ///	            get;
    ///	            set;
    ///	        }
    ///	
    ///	        public override IEnumerable&lt;Student> BindedObject
    ///	        {
    ///	            get;
    ///	            set;
    ///	        }
    ///	
    ///	        private StudentHandler handlerStudent = new StudentHandler();
    ///	        protected void Page_Load(object sender, EventArgs e)
    ///	        {
    ///	            LoadData();
    ///	        }
    ///	
    ///	        protected override void LoadData()
    ///	        {
    ///	            SetPageNumberPanel();
    ///	            GenericResponse&lt;IEnumerable&lt;Student>> response = handlerStudent
    ///	                .Include("StudentCourses.Course").GetAllGeneric(x => x.IsActive == true);
    ///	
    ///	            if (response.Successful)
    ///	            {
    ///	                this.PageNmbers.Text = PageNumberPanel;
    ///	                BindedObject = response.Result.OrderBy(x => x.Age).Skip(CurrentPage * PAGE_SIZE)
    ///	                    .Take(PAGE_SIZE);
    ///	                this.GridView1.DataSource = BindedObject;
    ///	                this.GridView1.DataBind();
    ///	            }
    ///	            else
    ///	                this.DisplayError(response);
    ///	
    ///	            this.StudentCombo1.DataBind();
    ///	        }
    ///	
    ///	        protected override void ResetData()
    ///	        {
    ///	            CurrentPage = 0;
    ///	            LoadData();
    ///	        }
    ///	
    ///	        public override void UpdateUI(MasIt.Common.FrontEnd.ControlModes ControlMode)
    ///	        {
    ///	            throw new NotImplementedException();
    ///	        }
    ///	
    ///	        protected void lnkbtnPrevious_Click(object sender, EventArgs e)
    ///	        {
    ///	            CurrentPage -= 1;
    ///	            LoadData();
    ///	        }
    ///	
    ///	        protected void lnkbtnNext_Click(object sender, EventArgs e)
    ///	        {
    ///	            CurrentPage += 1;
    ///	            LoadData();
    ///	        }
    ///	
    ///	        protected void ButtonRefresh_Click(object sender, EventArgs e)
    ///	        {
    ///	            ResetData();
    ///	        }
    ///	
    ///	        protected void ButtonSearch_Click(object sender, EventArgs e)
    ///	        {
    ///	            SetPageNumberPanel(x => x.Age > 10 && x.StudentId &lt; 300 && x.StudentCourses.Count() > 1);
    ///	
    ///	            GenericResponse&lt;IEnumerable&lt;Student>> response = handlerStudent
    ///	                .Include("StudentCourses.Course").GetAllGeneric(x => x.Age > 10 && x.StudentId &lt; 300 && x.StudentCourses.Count() > 1);
    ///	
    ///	            if (response.Successful)
    ///	            {
    ///	                this.PageNmbers.Text = PageNumberPanel;
    ///	                BindedObject = response.Result.OrderBy(x => x.Age).Skip(CurrentPage * PAGE_SIZE)
    ///	                    .Take(PAGE_SIZE);
    ///	                this.GridView1.DataSource = BindedObject;
    ///	                this.GridView1.DataBind();
    ///	            }
    ///	            else
    ///	                this.DisplayError(response);
    ///	        }
    ///	
    ///	        protected void ButtonUpdate_Click(object sender, EventArgs e)
    ///	        {
    ///	            UpdateRecord(BindedObject.FirstOrDefault());
    ///	        }
    ///	
    ///	        protected void ButtonSingle_Click(object sender, EventArgs e)
    ///	        {
    ///	            SelectSingle(1);
    ///	        }
    ///	
    ///	        protected void ButtonSms_Click(object sender, EventArgs e)
    ///	        {
    ///	            MAlertSSmsClient smsClient = new MAlertSSmsClient();
    ///	            if (smsClient.Send(smsClient.Create("this is a test", "94773887502", 3, 2)))
    ///	                this.DisplayError(new GenericResponse(true, "message sent"));
    ///	            else
    ///	                this.DisplayError(new GenericResponse(false, "message sent failed"));
    ///	        }
    ///	
    ///	        protected void ButtonEmail_Click(object sender, EventArgs e)
    ///	        {
    ///	            MAlertSNetEmailClient emailClient = new MAlertSNetEmailClient();
    ///	            if (emailClient.Send(emailClient.Create("niroshli@masholdings.com"
    ///	                , "Nirosh", "Hello this is a test message"
    ///	                , "Dear Nirosh, this is a test"
    ///	                , "")))
    ///	                this.DisplayError(new GenericResponse(true
    ///	                    , "email sent"));
    ///	            else
    ///	                this.DisplayError(new GenericResponse(false
    ///	                    , "email failed to sent"));
    ///	        }
    ///	
    ///	        protected void ButtonUpdate2_Click(object sender, EventArgs e)
    ///	        {
    ///	            UpdateRecord2(BindedObject.FirstOrDefault(x=>x.StudentId == 22));
    ///	        }
    ///	
    ///	        private void SetPageNumberPanel()
    ///	        {
    ///	            TotalItemCount = handlerStudent.CountGeneric();
    ///	            StringBuilder sb = new StringBuilder();
    ///	            if (CurrentPage == 0)
    ///	                PageNumberPanel = sb.Append("1, 2, 3, ..., ").Append(TotalItemCount / PAGE_SIZE).ToString();
    ///	            else
    ///	                PageNumberPanel = sb.Append("1, ..., ").Append(CurrentPage + 1).Append(", ").Append(CurrentPage + 2)
    ///	                    .Append(", ").Append(CurrentPage + 3).Append(", ..., ").Append(TotalItemCount / PAGE_SIZE).ToString();
    ///	        }
    ///	
    ///	        private void SetPageNumberPanel(Expression&lt;Func&lt;Student, bool>> searchExpression)
    ///	        {
    ///	            int totalCount = handlerStudent.CountGeneric(searchExpression);
    ///	            StringBuilder sb = new StringBuilder();
    ///	            if (totalCount &lt; PAGE_SIZE)
    ///	                PageNumberPanel = sb.Append("1...").ToString();
    ///	            else if (CurrentPage == 0)
    ///	                PageNumberPanel = sb.Append("1, 2, 3, ..., ").Append(totalCount / PAGE_SIZE).ToString();
    ///	            else
    ///	                PageNumberPanel = sb.Append("1, ..., ").Append(CurrentPage + 1).Append(", ").Append(CurrentPage + 2)
    ///	                    .Append(", ").Append(CurrentPage + 3).Append(", ..., ").Append(totalCount / PAGE_SIZE).ToString();
    ///	        }
    ///	
    ///	        private void ResetUserInterface(ControlModes controlMode)
    ///	        {
    ///	            switch (controlMode)
    ///	            {
    ///	                case ControlModes.NavDisabled: { this.ButtonPrevious.Enabled = false; this.ButtonNext.Enabled = false; } break;
    ///	                case ControlModes.NavEnabled: { this.ButtonNext.Enabled = true; this.ButtonPrevious.Enabled = true; } break;
    ///	                case ControlModes.NextDisabled: { this.ButtonNext.Enabled = false; this.ButtonPrevious.Enabled = true; } break;
    ///	                case ControlModes.PreviousDisabled: { this.ButtonPrevious.Enabled = false; this.ButtonNext.Enabled = true; } break;
    ///	            }
    ///	        }
    ///	
    ///	        private void UpdateRecord(Student student)
    ///	        {
    ///	            student.Age = 1;
    ///	            student.Description = "Editted on " + DateTime.Now;
    ///	
    ///	            Course course = new Course();
    ///	            course.EndDate = DateTime.Now;
    ///	            course.IsActive = false;
    ///	            course.IsWeekEnd = true;
    ///	            course.Name = "Newly added course on " + DateTime.Now;
    ///	            course.Period = 10;
    ///	            course.StartDate = DateTime.Now;
    ///	
    ///	            StudentCourse stdCourse = new StudentCourse();
    ///	            stdCourse.StudentId = student.StudentId;
    ///	            stdCourse.CourseId = course.CourseId;
    ///	            stdCourse.Course = course;
    ///	
    ///	            student.StudentCourses.Add(stdCourse);
    ///	
    ///	            GenericResponse&lt;Student> response = handlerStudent.UpdateGeneric(student);
    ///	
    ///	            //
    ///	            //student.Age = 120;
    ///	            //student.Description = "Editted on " + DateTime.Now;
    ///	            //GenericResponse&lt;Student> response = handlerStudent.UpdateGeneric(student, student.StudentId);
    ///	
    ///	            this.DisplayError(response);
    ///	        }
    ///	
    ///	        private void UpdateRecord2(Student student)
    ///	        {
    ///	            student.Age = 308;
    ///	            student.Description = "Editted on " + DateTime.Now;
    ///	
    ///	            student.StudentCourses.FirstOrDefault().Course.Period = 308;
    ///	            student.StudentCourses.FirstOrDefault().Course.Name = "Eddited on " + DateTime.Now;
    ///	
    ///	            student.StudentCourses.Remove(student.StudentCourses.LastOrDefault());
    ///	
    ///	            Course course = new Course();
    ///	            course.EndDate = DateTime.Now;
    ///	            course.IsActive = false;
    ///	            course.IsWeekEnd = true;
    ///	            course.Name = "Newly added course on " + DateTime.Now;
    ///	            course.Period = 10;
    ///	            course.StartDate = DateTime.Now;
    ///	
    ///	            StudentCourse stdCourse = new StudentCourse();
    ///	            stdCourse.StudentId = student.StudentId;
    ///	            stdCourse.CourseId = course.CourseId;
    ///	            stdCourse.Course = course;
    ///	
    ///	            student.StudentCourses.Add(stdCourse);
    ///	
    ///	            StudentHandler stdentHandler = new StudentHandler();
    ///	
    ///	            GenericResponse&lt;Student> response = stdentHandler
    ///	                .UpdateGraphGeneric(student
    ///	                , map => map.OwnedCollection(p => p.StudentCourses
    ///	                    , with => with.OwnedEntity(p => p.Course)));
    ///	
    ///	            // When the object is not attached errors comes then go with this option
    ///	            // student.Age = 120;
    ///	            // student.Description = "Editted on " + DateTime.Now;
    ///	            // GenericResponse&lt;Student> response = handlerStudent.AttachChild(course).AttachChild(stdCourse).UpdateGeneric(student);
    ///	
    ///	            // Want to update only the paramerters that were newly updated and you just want other properties untouched
    ///	            // Then go with this option, Once you pass the primary key the system think that you want it to find the
    ///	            // record from the DB and do only the update.
    ///	            // GenericResponse&lt;Student> response = handlerStudent.UpdateGeneric(student, student.StudentId);
    ///	
    ///	            this.DisplayError(response);
    ///	        }
    ///	
    ///	        private void SelectSingle(int p)
    ///	        {
    ///	            GenericResponse&lt;Student> response = handlerStudent.GetSingleGeneric(x => x.StudentId == p);
    ///	            this.DisplayError(response);
    ///	        }
    ///	    }
    ///	}
    /// </code></example>
    public abstract class HandlerBase<E, C> : DataRepository<C, E>, IDisposable, Nido.Common.BackEnd.IHandlerBase<E,C>
        where E : BaseObject, new()
        where C : BaseObjectConext, IBaseObjectConext, new()
    {
        private CachePolicy _cachePolicy;
        private IEnumerable<string> _tags;
        private bool _removedCache;
        private IEnumerable<E> removedList;
        /// <summary>
        /// Default constructor of the base handler class.
        /// </summary>
        public HandlerBase()
            : base(new C())
        {
            // initiate logging class           
            if (!isConfigured)
            {
                log4net.Config.XmlConfigurator.Configure();
                isConfigured = true;
            }
            logger = log4net.LogManager.GetLogger(this.LogPrefix);
        }

        /// <summary>
        /// User define constructor of the HandlerBase class
        /// </summary>
        /// <param name="includeList">List of include object. 
        /// This constructor can be used to set the default set of include objects. 
        /// So that respective handler will load the included objects by default.</param>
        public HandlerBase(Queue<string> includeList)
            : base(new C())
        {
            _includes = includeList;
        }

        /// <summary>
        /// Use this method to include all related tables or dataobjects.
        /// </summary>
        /// <param name="entityInclude">A string parameter of a related table object</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// Bll.Handlers.DriverHandler driverHandler = new Bll.Handlers.DriverHandler();
        /// RadGrid driverGrid = (RadGrid)ucDriver1.FindControl("tGrdDriver");
        /// 
        /// GenericResponse&lt;IEnumerable&lt;MasIt.Transport.Bll.Models.Driver>> response = 
        ///     driverHandler.Include("TransportDevision").Include("Vehicles")
        ///         .GetAllGeneric(x => x.TransportDevisionId == myStoredSession.TransportDivisionId);
        ///         
        /// if (response.Successful)
        /// {
        ///     driverGrid.DataSource = response.Result.ToList();
        ///     driverGrid.DataBind();
        ///     ucDriver1.ControlMode = MasIt.Common.FrontEnd.ControlModes.New;
        /// }
        /// else
        /// {
        ///     Response.Write(response.Messages[0]);
        /// }
        /// 
        /// StudentHandler handlerStudent = new StudentHandler();
        /// GenericResponse&lt;IEnumerable&lt;Student>> response = handlerStudent
        ///    .Include("StudentCourses.Course").GetAllGeneric();
        /// // The include given above not only include the Course table but also the
        /// // StudentCourses table too.
        /// </code></example>
        public HandlerBase<E, C> Include(string entityInclude)
        {
            if (!string.IsNullOrEmpty(entityInclude))
            {
                _includes.Enqueue(entityInclude);
            }

            return this;
        }

        /// <summary>
        /// Include related table before retrieving any models from the database.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public HandlerBase<E, C> Include(Expression<Func<E, object>> path)
        {
            if (path != null)
            {
                MemberExpression body = path.Body as MemberExpression;

                if (body == null)
                {
                    UnaryExpression ubody = (UnaryExpression)path.Body;
                    body = ubody.Operand as MemberExpression;
                }
                _includes.Enqueue(body.Member.Name);
            }
            return this;
        }

        /// <summary>
        /// Generic child table attached method. 
        /// This need to followed by the parent object attaching 
        /// as well as the corresponding UPDATE method call.
        /// </summary>
        /// <typeparam name="OtherE">The entity type to be attached</typeparam>
        /// <param name="entity">Entity object</param>
        /// <returns>The updated Handler base</returns>
        public HandlerBase<E, C> AttachChild<OtherE>(OtherE entity)
            where OtherE : BaseObject
        {
            var fulltypename = typeof(OtherE).AssemblyQualifiedName;
            if (fulltypename == null)
                throw new ArgumentException("Invalid Type passed to GetObjectSet!");
            _context.Set<OtherE>().Attach(entity);

            _context.Entry(entity).CurrentValues.SetValues(entity);
            // Set the state to modified.
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return this;
        }

        public HandlerBase<E, C> FromCache(CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)  
        {
            _cachePolicy = cachePolicy;
            _tags = tags;
            _removedCache = false;
            return this;
        }

        public HandlerBase<E, C> FromCacheFirstOrDefault(CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)
        {
            _cachePolicy = cachePolicy;
            _tags = tags;
            _removedCache = false;
            return this;
        }

        public HandlerBase<E, C> RemoveCache<TEntity>()
        {
            _removedCache = true;
            return this;
        }

        public HandlerBase<E, C> RemoveCache(out IEnumerable<E> removed)
        {
            _removedCache = true;
            removed = null;
            return this;
        }

        public HandlerBase<E, C> FromCache()
        {
            _removedCache = false;
         
            return this;
        }

        /// <summary>
        /// Use this method to properly handle Exception.
        /// This method will generate a Reference ID and log the error to a exception.log file with the reference ID.
        /// Then it will also create a message to the user with the error details and the refernce ID.
        /// </summary>
        /// <typeparam name="T">Expected return type of the method call</typeparam>
        /// <param name="e">BaseException occured</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// public GenericResponse&lt;IEnumerable&lt;Driver>> GetAllDeep(Func{Driver, bool} predicate)
        /// {
        ///     try
        ///     {
        ///         return SelectSuccessResponse(this._context.Drivers.Include("TransportDevision")
        ///             .Where(predicate).ToList());
        ///     }
        ///     catch (ExceptionBase e)
        ///     {
        ///        return HandleException{E}(e);
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return HandleException{IEnumerable{Driver}}(e);
        ///     }
        /// }
        /// </code></example>
        protected GenericResponse<T> HandleException<T>(ExceptionBase e)
            where T : class
        {
            StackTrace stackTrace = new StackTrace();
            GenericResponse<T> gResponse = new GenericResponse<T>();
            gResponse.Title = stackTrace.GetFrame(1).GetMethod().Name;
            this.LogError(e.RefId, e);
            gResponse.Successful = false;
            gResponse.Messages = new List<string>() { "Error Occured!! Ref Id: " + e.RefId + ". Please consult support team with this error Id", e.UserErrorMessage };
            gResponse.Error1 = e.Message;
            gResponse.Error2 = (e.InnerException != null) ? e.InnerException.Message : "";
            gResponse.CurrentException = e;
            return gResponse;
        }

        /// <summary>
        /// Use this method to properly handle Exception.
        /// This method will generate a Reference ID and log the error to a exception.log file with the reference ID.
        /// Then it will also create a message to the user with the error details and the refernce ID.
        /// </summary>
        /// <typeparam name="T">Expected return type of the method call</typeparam>
        /// <param name="e">Exception occured</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// public GenericResponse&lt;IEnumerable&lt;Driver>> GetAllDeep(Func&lt;Driver, bool> predicate)
        /// {
        ///     try
        ///     {
        ///         return SelectSuccessResponse(this._context.Drivers.Include("TransportDevision")
        ///             .Where(predicate).ToList());
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return HandleException&lt;IEnumerable&lt;Driver>>(e);
        ///     }
        /// }
        /// </code></example>
        protected GenericResponse<T> HandleException<T>(Exception e)
            where T : class
        {
            GenericResponse<T> gResponse = new GenericResponse<T>();
            string id = ExceptionCritical.GetRefId();
            this.LogError(id, e);
            gResponse.Successful = false;
            gResponse.Messages = new List<string>() { "Error Occured!! Ref Id: " + id + ". Please consult support team with this error Id", e.Message };
            gResponse.Error1 = e.Message;
            gResponse.Error2 = (e.InnerException != null) ? e.InnerException.Message : "";
            gResponse.CurrentException = e;
            return gResponse;
        }

        /// <summary>
        /// Get a response after updating a object to the database successfully
        /// </summary>
        /// <param name="entity">The updated object </param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> UpdateSuccessResponse(E entity)
        {
            return new GenericResponse<E>(entity, true, ExceptionBase.GetName<E>(entity) + " record successfully updated !");
        }

        /// <summary>
        /// Update failling response.
        /// </summary>
        /// <param name="entity">The object tried to update</param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> UpdateFailResponse(E entity)
        {
            return new GenericResponse<E>(entity, false, "An Error prevented from updating the record. " + ExceptionBase.GetName<E>(entity) + " fail to update.");
        }

        /// <summary>
        /// Get the add success message after 
        /// successfully add a new object to the database.
        /// </summary>
        /// <param name="entity">The added object</param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> AddSuccessResponse(E entity)
        {
            return new GenericResponse<E>(entity, true, ExceptionBase.GetName<E>(entity) + " record successfully added !");
        }

        /// <summary>
        /// Get the add success message after 
        /// fail to add a new object to the database.
        /// </summary>
        /// <param name="entity">The failed object</param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> AddFailResponse(E entity)
        {
            return new GenericResponse<E>(entity, false, "An Error prevented from adding the record. " + ExceptionBase.GetName<E>(entity) + " fail to add.");
        }

        /// <summary>
        /// Get the delete success message after 
        /// successfully deleting a new object from the database.
        /// </summary>
        /// <param name="entity">The deleted object</param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> DeleteSuccessResponse(E entity)
        {
            return new GenericResponse<E>(entity, true, ExceptionBase.GetName<E>(entity) + " record successfully deleted !");
        }

        /// <summary>
        /// Get the delete failed message after 
        /// fail to delete an object from the database.
        /// </summary>
        /// <param name="entity">The delete tried object</param>
        /// <returns>Generic response</returns>
        protected GenericResponse<E> DeleteFailResponse(E entity)
        {
            return new GenericResponse<E>(entity, false, "An error prevented from deleting the record. " + ExceptionBase.GetName<E>(entity) + " fail to Delete.");
        }

        /// <summary>
        /// Use this method to properly select a set of objects from the database and return it to the UI layer.
        /// </summary>
        /// <param name="list">List of objects that are being selected</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// public GenericResponse&lt;IEnumerable&lt;Driver>> GetAllDeep(Func&lt;Driver, bool> predicate)
        /// {
        ///     try
        ///     {
        ///         return SelectSuccessResponse(this._context.Drivers.Include("TransportDevision")
        ///             .Where(predicate).ToList());
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return HandleException&lt;IEnumerable&lt;Driver>>(e);
        ///     }
        /// }
        /// </code></example>
        // [Obsolete("SelectSuccessResponse(IEnumerable<E> list) is deprecated, please use SelectSuccessResponse(IQueryable<E> list) instead.")]
        protected static GenericResponse<IEnumerable<E>> SelectSuccessResponse(IEnumerable<E> list)
        {
            if (list != null)
            {
                return new GenericResponse<IEnumerable<E>>(list, true, "The list of " + ExceptionBase.GetName<E>(null) + " loaded successfully");
            }
            else
            {
                return new GenericResponse<IEnumerable<E>>(list, false, "Failed to load the " + ExceptionBase.GetName<E>(null) + " list.");
            }
        }

        /// <summary>
        /// Use this method to properly select a set of objects from the database and return it to the UI layer.
        /// </summary>
        /// <param name="list">List of objects that are being selected</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// public GenericResponse&lt;IQueryable&lt;Driver>> GetAllDeep(Func&lt;Driver, bool> predicate)
        /// {
        ///     try
        ///     {
        ///         return SelectSuccessResponse(this._context.Drivers.Include("TransportDevision")
        ///             .Where(predicate).ToList());
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return HandleException&lt;IQueryable&lt;Driver>>(e);
        ///     }
        /// }
        /// </code></example>
        protected static GenericResponse<IQueryable<E>> SelectSuccessResponse(IQueryable<E> list)
        {
            if (list != null)
            {
                return new GenericResponse<IQueryable<E>>(list, true, "The list of " + ExceptionBase.GetName<E>(null) + " loaded successfully");
            }
            else
            {
                return new GenericResponse<IQueryable<E>>(list, false, "Failed to load the " + ExceptionBase.GetName<E>(null) + " list.");
            }
        }

        /// <summary>
        /// Use this method to properly select a set of objects from the database and return it to the UI layer.
        /// </summary>
        /// <param name="entity">The object selected</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// public GenericResponse&lt;IEnumerable&lt;Driver>> GetAllDeep(Func&lt;Driver, bool> predicate)
        /// {
        ///     try
        ///     {
        ///         return SelectSuccessResponse(this._context.Drivers.Include("TransportDevision")
        ///             .Where(predicate).ToList());
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return HandleException&lt;IEnumerable&lt;Driver>>(e);
        ///     }
        /// }
        /// </code></example>
        protected static GenericResponse<E> SelectSuccessResponse(E entity)
        {
            return new GenericResponse<E>(entity, true, "The record of " + ExceptionBase.GetName<E>(entity) + " loaded successfully");
        }

        /// <summary>
        /// Get the select fail  message after 
        /// fail to select object(s) from the database.
        /// </summary>
        /// <returns>Generic response</returns>
        protected static GenericResponse<E> SelectFailResponse()
        {
            return new GenericResponse<E>(null, false, "Failed to load the " + ExceptionBase.GetName<E>(null) + " record set.");
        }

        /// <summary>
        /// Add a Database object directly to the database.
        /// You can use this method directly on the UI layer.
        /// This method does not throw any exceptions.
        /// </summary>
        /// <param name="entity">Repective object</param>
        /// <returns></returns>
        /// <remarks>You need to override the 'ValidateForAdd' method. 
        /// This way you can instruct the system to validate the entity before the Add</remarks>
        /// <example><code lang="C#">
        /// 
        /// Bll.Handlers.DriverHandler driverHandler = new Bll.Handlers.DriverHandler();
        /// Driver driver = new Driver();
        /// TODO: Set the values of the driver object as required
        /// 
        /// GenericResponse&lt;MasIt.Transport.Bll.Models.Driver> response = 
        ///     driverHandler.AddGeneric(driver);
        ///
        /// Response.Write(response.Messages[0]);
        /// 
        /// </code></example>
        public virtual GenericResponse<E> AddGeneric(E entity)
        {
            try
            {
                this.LogInfo("Calling start for AddGeneric(" + entity.ToString() + ")");
                this.LogEntityInfo(entity);
                string[] validationErrors;
                if (ValidateForUpdate(entity, out validationErrors))
                {
                    this.AddNew(entity);
                    if (this.SaveChanges() > 0)
                    {
                        this.LogInfo("Calling End Successfully for AddGeneric(" + entity.ToString() + ")");
                        return AddSuccessResponse(entity);
                    }
                }
                this.LogInfo("Calling failed at save for AddGeneric(" + entity.ToString() + ")");
                GenericResponse<E> response = AddFailResponse(entity); ;
                response.Messages.AddRange(validationErrors);
                return response;
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// Add a set of entities to the database quickly.
        /// </summary>
        /// <remarks>This method does not validate records and 
        /// if one record is wrong then all the records may fail.</remarks>
        /// <param name="entities">Th set of records to be added to the database</param>
        /// <returns>Operation status with success, error messages</returns>
        public virtual GenericResponse<E> AddBulkGeneric(IEnumerable<E> entities)
        {
            try
            {
                this.LogInfo("Calling start for AddBulkGeneric(" + entities.ToString() + " )");

                _context.Configuration.AutoDetectChangesEnabled = false;
                _context.Configuration.ValidateOnSaveEnabled = false;

                this.AddBulkNew(entities);
                if (this.SaveChanges() > 0)
                    this.LogInfo("Calling End Successfully for AddGeneric(" + entities.ToString() + ")");
                else
                    this.LogInfo("Calling failed at save for AddGeneric(" + entities.ToString() + ")");

                return new GenericResponse<E>(true, new string[] { "Bulk Insert operation is successfull" });
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                return HandleException<E>(e);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
                _context.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        /// <summary>
        /// Add a object to the database
        /// </summary>
        /// <param name="entity">the object to be added to the database</param>
        /// <returns>The added object with the **updated** primary key.</returns>
        [Obsolete("Add(E entity) is deprecated, please use AddGeneric(E entity) instead.")]
        public virtual E Add(E entity)
        {
            try
            {
                this.AddNew(entity);
                if (this.SaveChanges() > 0)
                    return entity;
                else
                    return null;
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw e;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// Delete a Database object from the database given the object, which retrieved from the database.
        /// You can use this method directly on the UI layer.
        /// This method does not throw any exceptions.
        /// </summary>
        /// <param name="entity">Repective object to be removed from the database</param>
        /// <remarks>Please implement the 'CanDelete' property of the object.
        /// This method will autoamtically validate the operation against the 'CanDelete' property 
        /// and avoid deleting it if CanDelete = false.</remarks>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// 
        /// Bll.Handlers.DriverHandler driverHandler = new Bll.Handlers.DriverHandler();
        /// Driver driver = new Driver();
        /// TODO: Set the values of the driver object as required
        /// 
        /// GenericResponse&lt;MasIt.Transport.Bll.Models.Driver> response = 
        ///     driverHandler.DeleteGeneric(driver);
        ///
        /// Response.Write(response.Messages[0]);
        /// 
        /// </code></example>
        public virtual GenericResponse<E> DeleteGeneric(E entity)
        {
            try
            {
                if (entity.CanDelete)
                {
                    this.DeleteNew(entity);
                    if (this.SaveChanges() > 0)
                        return DeleteSuccessResponse(entity);
                    return DeleteFailResponse(entity);
                }
                return new GenericResponse<E>(entity, false
                    , "The database depandancies prevented the " + ExceptionBase.GetName<E>(entity) + " record deletion.");
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                return HandleException<E>(e);
            }
        }

        /// <summary>
        /// Delete a Database object from the database given the expression to find the object(s).
        /// You can use this method directly on the UI layer.
        /// This method does not throw any exceptions.
        /// </summary>
        public virtual GenericResponse<E> DeleteGeneric(Expression<Func<E, bool>> filterExpression)
        {
            try
            {
                if (this.DeleteNew(filterExpression) > 0)
                    return DeleteSuccessResponse(null);
                return DeleteFailResponse(null);
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                return HandleException<E>(e);
            }
        }

        /// <summary>
        /// Delete a Database object from the database given the primary key of the particular object.
        /// You can use this method directly on the UI layer.
        /// This method does not throw any exceptions.
        /// </summary>
        /// <param name="E">Repective object to be removed from the database</param>
        /// <remarks>Please implement the 'CanDelete' property of the object.
        /// This method will autoamtically validate the operation against the 'CanDelete' property 
        /// and avoid deleting it if CanDelete = false.</remarks>
        /// <param name="entityId">The entity key to find the object from the database</param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// 
        /// Bll.Handlers.DriverHandler driverHandler = new Bll.Handlers.DriverHandler();
        /// int entityId = GetDriverIdFromUI();
        /// 
        /// GenericResponse&lt;MasIt.Transport.Bll.Models.Driver> response = 
        ///     driverHandler.DeleteGeneric(entityId);
        ///
        /// Response.Write(response.Messages[0]);
        /// 
        /// </code></example>
        public virtual GenericResponse<E> DeleteGeneric(int entityId)
        {
            try
            {
                this.LogInfo("Calling start for DeleteGeneric(" + entityId + ")");
                E entity = GetObjectSet().Find(entityId);
                this.DeleteNew(entity);
                if (this.SaveChanges() > 0)
                {
                    this.LogInfo("Calling End Successfully for DeleteGeneric(" + entityId + ")");
                    this.LogEntityInfo(entity);
                    return DeleteSuccessResponse(entity);
                }
                this.LogInfo("Calling failed at save for DeleteGeneric(" + entityId + ")");
                return DeleteFailResponse(entity);
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                return HandleException<E>(e);
            }
        }

        /// <summary>
        /// Delete a object from the database.
        /// The save method is called.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <returns></returns>
        [Obsolete("Delete(E entity) is deprecated, please use DeleteGeneric(E entity) instead.")]
        public virtual bool Delete(E entity)
        {
            try
            {
                if (entity.CanDelete)
                {
                    this.DeleteNew(entity);
                    if (this.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                this.LogInfo("The object cannot be deleted");
                return false;
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// Get the matching record from the database. 
        /// The method support calling with includes directly on the UI Layer.
        /// If you are selecting by primary key, GetSingleGeneric is semantically the correct methods to call 
        /// and is usually quite fast.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// 
        /// EmployeeHandler handler = new EmployeeHandler(); 
        /// GenericResponse&lt;IEnumerable&lt;Employee>> response = handler.Include("Grade").Include("Requisitions")
        ///         .GetSingleGeneric(x => x.EmployeeId == 3153);
        ///         
        /// </code></example>
        public virtual GenericResponse<E> GetSingleGeneric(Expression<Func<E, Boolean>> predicate)
        {
            try
            {
                this.LogInfo("Calling start for GetSingleGeneric(" + predicate.ToString() + ")");
                E entity = this.GetSingleNew(predicate);
                if (entity != null)
                {
                    this.LogInfo("Calling End Successfully for GetSingleGeneric(" + predicate.ToString() + ")");
                    this.LogEntityInfo(entity);
                    return SelectSuccessResponse(entity);
                }
                this.LogInfo("Calling End with empty records for GetSingleGeneric(" + predicate.ToString() + ")");
                return SelectFailResponse();
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with errors for GetSingleGeneric(" + predicate.ToString() + ")");
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with errors for GetSingleGeneric(" + predicate.ToString() + ")");
                return HandleException<E>(e);
            }
        }

        /// <summary>
        /// Get the first matching record from the database. 
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <param name="predicate">condtional statement</param>
        /// <returns>GenericReponse</returns>
        public virtual GenericResponse<E> GetFirstGeneric(Expression<Func<E, Boolean>> predicate)
        {
            try
            {
                this.LogInfo("Calling start for GetFirstGeneric(" + predicate.ToString() + ")");
                E entity = this.GetFirstNew(predicate);
                if (entity != null)
                {
                    this.LogInfo("Calling End Successfully for GetFirstGeneric(" + predicate.ToString() + ")");
                    this.LogEntityInfo(entity);
                    return SelectSuccessResponse(entity);
                }
                this.LogInfo("Calling End with empty records for GetFirstGeneric(" + predicate.ToString() + ")");
                return SelectFailResponse();
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with errors for GetFirstGeneric(" + predicate.ToString() + ")");
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with erros for GetFirstGeneric(" + predicate.ToString() + ")");
                return HandleException<E>(e);
            }
        }

        /// <summary>
        /// Get all the matching objects from the database. 
        /// The method support calling with includes directly on the UI Layer. 
        /// </summary>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// 
        /// EmployeeHandler handler = new EmployeeHandler(); 
        /// GenericResponse&lt;IEnumerable&lt;Employee>> response = handler.Include("Grade").Include("Requisitions")
        ///     .GetAllGeneric(x => x.EmployeeId == 3153);
        ///     
        /// </code></example>
        public virtual GenericResponse<IEnumerable<E>> GetAllGeneric(Expression<Func<E, Boolean>> predicate)
        {
            this.LogInfo("Calling start for GetAllGeneric(" + predicate.ToString() + ")");
            GenericResponse<IEnumerable<E>> gResponse = new GenericResponse<IEnumerable<E>>();
            try
            {
                IEnumerable<E> list = this.GetAllNew(predicate);
                this.LogInfo("Calling End Successfully for GetAllGeneric(" + predicate.ToString() + ")");
                return SelectSuccessResponse(list);
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with errors for GetAllGeneric(" + predicate.ToString() + ")");
                return HandleException<IEnumerable<E>>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with errors for GetAllGeneric(" + predicate.ToString() + ")");
                return HandleException<IEnumerable<E>>(e);
            }
        }

        /// <summary>
        /// Get all the matching objects from the database and cache the query. 
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <returns></returns>
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
        public virtual GenericResponse<IEnumerable<E>> GetAllGenericFromCache(Expression<Func<E, Boolean>> predicate
            , CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)
        {
            
            this.LogInfo("Calling start for GetAllGeneric(" + predicate.ToString() + ")");
            GenericResponse<IEnumerable<E>> gResponse = new GenericResponse<IEnumerable<E>>();
            try
            {
                IEnumerable<E> list = this.GetAllFromCache(predicate, cachePolicy, tags);
                this.LogInfo("Calling End Successfully for GetAllGeneric(" + predicate.ToString() + ")");
                return SelectSuccessResponse(list);
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with errors for GetAllGeneric(" + predicate.ToString() + ")");
                return HandleException<IEnumerable<E>>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with errors for GetAllGeneric(" + predicate.ToString() + ")");
                return HandleException<IEnumerable<E>>(e);
            }
        }

        /// <summary>
        /// Get the object with_out including other related objects. 
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <returns></returns>
        /// <example><code lang="C#">
        /// 
        /// EmployeeHandler handler = new EmployeeHandler(); 
        /// GenericResponse&lt;IEnumerable&lt;Employee>> response = handler.Include("Grade").Include("Requisitions")
        ///         .GetAllGeneric(x => x.EmployeeId == 3153);
        ///         
        /// </code></example>
        public virtual GenericResponse<IEnumerable<E>> GetAllGeneric()
        {
            this.LogInfo("Calling start for GetAllGeneric()");
            GenericResponse<IEnumerable<E>> gResponse = new GenericResponse<IEnumerable<E>>();
            try
            {
                IEnumerable<E> list = this.GetAllNew().AsEnumerable();
                this.LogInfo("Calling End Successfully for GetAllGeneric()");
                return SelectSuccessResponse(list);
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with erros for GetAllGeneric()");
                return HandleException<IEnumerable<E>>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with erros for GetAllGeneric()");
                return HandleException<IEnumerable<E>>(e);
            }
        }

        /// <summary>
        /// Get the object with_out including other related objects and cache the query. 
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <returns></returns>
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
        public virtual GenericResponse<IEnumerable<E>> GetAllGenericFromCache(CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)
        {
            this.LogInfo("Calling start for GetAllGeneric()");
            GenericResponse<IEnumerable<E>> gResponse = new GenericResponse<IEnumerable<E>>();
            try
            {
                IEnumerable<E> list = this.GetAllFromCache(cachePolicy, tags).AsEnumerable();
                this.LogInfo("Calling End Successfully for GetAllGeneric()");
                return SelectSuccessResponse(list);
            }
            catch (ExceptionBase e)
            {
                this.LogInfo("Calling End with erros for GetAllGeneric()");
                return HandleException<IEnumerable<E>>(e);
            }
            catch (Exception e)
            {
                this.LogInfo("Calling End with erros for GetAllGeneric()");
                return HandleException<IEnumerable<E>>(e);
            }
        }

        /// <summary>
        /// Get all the object as enumerable from the database
        /// </summary>
        /// <returns></returns>
        [Obsolete("GetAll() is deprecated, please use GetAllGeneric() or GetAllGeneric(Func<E, bool> predicate) instead.")]
        public virtual IEnumerable<E> GetAll()
        {
            try
            {
                return this.GetAllNew();
                //transportEntity
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// When the Entities are in detached mode. This method automatically update a graph of entities
        /// , that is updating a record with multiple sb entities done in a seemless manner.
        /// </summary>
        /// <param name="entity">The primary entity object that you wanted to update</param>
        /// <param name="mapping">The mapping description of all the other related objects</param>
        /// <returns>Generic response indicating the success/ failure, errors, exceptions, and user messages </returns>
        /// <remarks>This is a very powerful method, So I urge you to use it visely,
        /// otherwise it can hit you back with system performance issues</remarks>
        /// <example><code lang="C#">
        /// 
        /// private void UpdateRecord2(Student student)
        /// {
        ///     student.Age = 12;
        ///     student.Description = "Editted on " + DateTime.Now;
        ///
        ///     Course course = new Course();
        ///     course.EndDate = DateTime.Now;
        ///     course.IsActive = false;
        ///     course.IsWeekEnd = true;
        ///     course.Name = "Newly added course on " + DateTime.Now;
        ///     course.Period = 10;
        ///     course.StartDate = DateTime.Now;
        ///
        ///     StudentCourse stdCourse = new StudentCourse();
        ///     stdCourse.StudentId = student.StudentId;
        ///     stdCourse.CourseId = course.CourseId;
        ///     stdCourse.Course = course;
        ///
        ///     student.StudentCourses.Add(stdCourse);
        ///
        ///     StudentHandler stdentHandler = new StudentHandler();
        ///
        ///     GenericResponse&lt;Student> response = stdentHandler.UpdateGraphGeneric(student
        ///         , map => map.OwnedCollection(p => p.StudentCourses
        ///             , &lt; => with.AssociatedEntity(p => p.Course)));
        ///
        ///     // When the object is not attached errors comes then go with this option
        ///     // student.Age = 120;
        ///     // student.Description = "Editted on " + DateTime.Now;
        ///     // GenericResponse&lt;Student> response = handlerStudent.AttachChild(course).AttachChild(stdCourse).UpdateGeneric(student);
        ///
        ///     // Want to update only the paramerters that were newly updated and you just want other properties untouched
        ///     // Then go with this option, Once you pass the primary key the system think that you want it to find the
        ///     // record from the DB and do only the update.
        ///     // GenericResponse&lt;Student> response = handlerStudent.UpdateGeneric(student, student.StudentId);
        ///     this.DisplayError(response);
        /// } 
        ///
        /// </code></example>
        public virtual GenericResponse<E> UpdateGraphGeneric(E entity
            , Expression<Func<IUpdateConfiguration<E>, object>> mapping)
        {
            try
            {
                this.LogInfo("Calling Start UpdateGraphGeneric");
                if (!_context.EnableOptTracking)
                {
                    LogEntityInfo(entity);
                    this.LogInfo(mapping.ToString());
                }

                string[] validationErrors;
                if (ValidateForUpdate(entity, out validationErrors))
                {
                    this.LogInfo("Record is Validated");
                    _context.UpdateGraph(entity, mapping);
                    if (_context.SaveChanges() > 0)
                    {
                        this.LogInfo("Calling End UpdateGraphGeneric after Saving Changes");
                        return UpdateSuccessResponse(entity);
                    }
                }

                GenericResponse<E> response = UpdateFailResponse(entity);
                response.Messages.AddRange(validationErrors);

                this.LogInfo("Calling End UpdateGraphGeneric without Saving Changes");
                return response;
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// Update the entity after merging that with the record found in the database.
        /// Merging done while giving priority to the database record
        /// 1. If null found then replace that with the value found in the DB record
        /// 2. If 0 found then replace that with the valus found in the DB record
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <param name="entity">The record with updated fields, primary key and all the reuqired foreign keys</param>
        /// <param name="entityId">Primary key of the respective entity</param>
        /// <returns></returns>
        /// <remarks>You need to override the 'ValidateForUpdate' method. 
        /// This way you can instruct the system to validate the entity before the update</remarks>
        /// <example><code lang="C#">
        /// 
        ///     Employee p1 = new Employee();
        ///     p1.EmployeeId = 2250; // Primary Key. This is a Rqquired field
        ///     p1.SubUnitId = 10;
        ///     p1.UserId = "12";
        ///     p1.Name = "Owen";
        ///     EmployeeHandler handler = new EmployeeHandler();
        ///     
        ///     handler.UpdateGeneric(p1, p1.EmployeeId);
        ///     
        /// </code></example>
        public virtual GenericResponse<E> UpdateGeneric(E entity, int entityId)
        {
            try
            {
                this.LogInfo("Calling Start UpdateGeneric with entity Id " + entityId);
                if (!_context.EnableOptTracking)
                    LogEntityInfo(entity);

                E dbEntity = this.GetObjectSet().Find(entityId);
                CompareLogic compare = new CompareLogic();
                compare.Config.MaxDifferences = Convert.ToInt32(ConfigSettings.ReadConfigValue("MaxModelRecordCount", "100"));
                compare.Config.AttributesToIgnore.Add(typeof(NotMappedAttribute));
                compare.Config.AttributesToIgnore.Add(typeof(InversePropertyAttribute));
                ComparisonResult result = compare.Compare(entity, dbEntity);
                foreach (Difference item in result.Differences)
                {
                    if ((item.Object1Value == "(null)")
                        || (item.Object1Value == "0")
                        || (string.IsNullOrEmpty(item.Object1Value)))
                    {
                        PropertyInfo pInfoMBy = dbEntity.GetType().GetProperty(item.PropertyName.TrimStart('.'));
                        if (pInfoMBy != null)
                            pInfoMBy.SetValue(entity, Convert.ChangeType(item.Object2Value, pInfoMBy.PropertyType), null);

                        this.LogInfo("Comparer Found These Differences: " + result.DifferencesString);
                    }
                }
                this.LogInfo("Calling End UpdateGeneric with entity Id " + entityId + " Diverting the call the standard update generic method");

                return UpdateGeneric(entity);
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// The record in the DB is updated/ override with the given new records. 
        /// The method support calling with includes directly on the UI Layer.
        /// </summary>
        /// <param name="entity">The record with updated fields, primary key and all the reuqired foreign keys </param>
        /// <returns></returns>
        /// <remarks>You need to override the 'ValidateForUpdate' method. 
        /// This way you can instruct the system to validate the entity before the update</remarks>
        /// <example><code lang="C#">
        /// 
        ///     Employee p1 = new Employee();
        ///     p1.EmployeeId = 2250; // Primary Key. This is a Rqquired field
        ///     p1.SubUnitId = 10;
        ///     p1.UserId = "12";
        ///     p1.Name = "Owen";
        ///     EmployeeHandler handler = new EmployeeHandler();
        ///     
        ///     handler.UpdateGeneric(p1);
        ///     
        /// </code></example>
        public virtual GenericResponse<E> UpdateGeneric(E entity)
        {
            try
            {
                this.LogInfo("Calling Start UpdateGeneric");
                if (!_context.EnableOptTracking)
                    LogEntityInfo(entity);
                string[] validationErrors;
                if (ValidateForUpdate(entity, out validationErrors))
                {
                    this.LogInfo("Record is Validated");
                    if (this.UpdateNew(entity))
                        if (this.SaveChanges() > 0)
                        {
                            this.LogInfo("Calling End UpdateGeneric after Saving Changes");
                            return UpdateSuccessResponse(entity);
                        }
                }
                GenericResponse<E> response = UpdateFailResponse(entity);

                response.Messages.AddRange(getValidationErrors());

                this.LogInfo("Calling End UpdateGeneric without Saving Changes");
                return response;
            }
            catch (ExceptionBase e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// Update the DB record using a update expression.
        /// Help:https://github.com/loresoft/EntityFramework.Extended
        /// </summary>
        /// <param name="updateExpression">Expression that direct the way to update the record</param>
        /// <returns>Generic Response</returns>
        /// <example>
        /// <code lang="c#">
        /// // example of using an IQueryable as the filter for the update
        /// context.Users.Update(u => new User {FirstName = "newfirstname"});
        /// </code>
        /// </example>
        public virtual GenericResponse<E> UpdateGeneric(Expression<Func<E, E>> updateExpression)
        {
            try
            {
                this.LogInfo("Calling Start UpdateGeneric");
                GenericResponse<E> response = new GenericResponse<E>();
                this.LogInfo("Record not is Validated");
                if (this.UpdateNew(updateExpression))
                    if (this.SaveChanges() >= 0)
                    {
                        this.LogInfo("Calling End UpdateGeneric after Saving Changes");
                        response.Successful = true;
                        response.Messages.Add("Successfully update the records");
                    }
                    else
                    {
                        response.Successful = false;
                        response.Messages.Add("Failed to update the records");
                    }
                else
                {
                    response.Successful = false;
                    response.Messages.Add("Failed to update the records");
                }
                return response;
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// Update the DB record using a filtering and update expression.
        /// Help:https://github.com/loresoft/EntityFramework.Extended
        /// </summary>
        /// <param name="filterExpression">Expressin that uses to filter the records that needed to be updated</param>
        /// <param name="updateExpression">Expression that direct the way to update the record</param>
        /// <returns>Generic Response</returns>
        /// <example>
        /// <code lang="c#">
        /// // update all tasks with status of 1 to status of 2
        /// taskHanlder.Update(
        ///     t => t.StatusId == 1, 
        ///     t2 => new Task {StatusId = 2});
        /// 
        /// </code>
        /// </example>
        public virtual GenericResponse<E> UpdateGeneric(Expression<Func<E, bool>> filterExpression, Expression<Func<E, E>> updateExpression)
        {
            try
            {
                this.LogInfo("Calling Start UpdateGeneric");
                GenericResponse<E> response = new GenericResponse<E>();
                this.LogInfo("Record not is Validated");
                if (this.UpdateNew(filterExpression, updateExpression))
                    if (this.SaveChanges() >= 0)
                    {
                        this.LogInfo("Calling End UpdateGeneric after Saving Changes");
                        response.Successful = true;
                        response.Messages.Add("Successfully update the records");
                    }
                    else
                    {
                        response.Successful = false;
                        response.Messages.Add("Failed to update the records");
                    }
                else
                {
                    response.Successful = false;
                    response.Messages.Add("Failed to update the records");
                }
                return response;
            }
            catch (ExceptionBase e)
            {
                return HandleException<E>(e);
            }
            catch (Exception e)
            {
                GenericResponse<E> response1 = HandleException<E>(e);
                response1.Messages.AddRange(getValidationErrors());
                return response1;
            }
        }

        /// <summary>
        /// Update a object to the database.
        /// </summary>
        /// <param name="entity">The object to be updated</param>
        /// <returns></returns>
        [Obsolete("Update(E entity) is deprecated, please use UpdateGeneric(E entity) instead.")]
        public virtual bool Update(E entity)
        {
            try
            {
                this.UpdateNew(entity);
                if (this.SaveChanges() > 0)
                    return true;
                else
                    return false;
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// Get the total number of records of the respective EntitySet.
        /// </summary>
        /// <returns>Integer value indicating the total number of records in the database.</returns>
        /// <example><code lang="C#">
        /// StudentHandler handlerStudent = new StudentHandler();
        /// int totalCount = handlerStudent.CountGeneric();
        /// if (totalCount < PAGE_SIZE)
        /// {
        ///     // Do your coding here
        /// }
        /// </code>
        /// </example>
        public int CountGeneric()
        {
            try
            {
                this.LogInfo("Calling Start CountGeneric: no params");
                int i = this.CountNew();
                this.LogInfo("Ending Start CountGeneric: no params");
                return i;
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// Filter EntitySet base on the given criteria and get the number of records
        /// of the respective EntitySet.
        /// </summary>
        /// <returns>Integer value indicating the total number of records in the database.</returns>
        /// <example><code lang="C#">
        /// StudentHandler handlerStudent = new StudentHandler();
        /// int totalCount = handlerStudent.CountGeneric(x => x.Age > 18 
        /// && x.StudentId < 300 
        /// && x.StudentCourses.Count() > 1);
        /// 
        /// if (totalCount < PAGE_SIZE)
        /// {
        ///     // Do your coding here
        /// }
        /// </code>
        /// </example>
        public int CountGeneric(Expression<Func<E, Boolean>> predicate)
        {
            try
            {
                this.LogInfo("Calling Start CountGeneric: " + predicate.ToString());
                int i = this.CountNew(predicate);
                this.LogInfo("Ending End CountGeneric: " + predicate.ToString());
                return i;
            }
            catch (ExceptionDataError e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionNoneRepeatable e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (ExceptionCritical e)
            {
                this.LogError(e.RefId, e);
                throw;
            }
            catch (Exception e)
            {
                string id = ExceptionCritical.GetRefId();
                this.LogError(id, e);
                throw new ExceptionCritical(e, id);
            }
        }

        /// <summary>
        /// Override this method to instruct the system to validate entity before an Update
        /// </summary>
        /// <param name="entity">The entity to be validated. 
        /// This parameter is normally passed by the respective base handler's method.</param>
        /// <param name="errors">Validation errors. 
        /// These will be passed back to the UI layer as Messages of the GenericResponse object</param>
        /// <returns></returns>
        public virtual bool ValidateForUpdate(E entity
            , out string[] errors)
        {
            errors = new string[] { "" };
            return true;
        }

        /// <summary>
        /// Override this method to instruct the system to validate entity before an Add
        /// </summary>
        /// <param name="entity">The entity to be validated. 
        /// This parameter is normally passed by the respective base handler's method.</param>
        /// <param name="errors">Validation errors. 
        /// These will be passed back to the UI layer as Messages of the GenericResponse object</param>
        /// <returns></returns>
        public virtual bool ValidateForAdd(E entity
            , out string[] errors)
        {
            errors = new string[] { "" };
            return true;
        }

        #region LoggerBase
        #region Member Variables

        /// <summary>
        /// Member variable to hold the instance.
        /// </summary>
        private readonly log4net.ILog logger = null;

        #endregion

        #region Properties

        /// <summary>
        /// Abstract property which must be overridden by the derived classes.
        /// The logger prefix is used to create the logger instance.
        /// </summary>
        protected abstract System.Type LogPrefix
        {
            get;
        }

        #endregion

        #region Constructors

        private static bool isConfigured = false;

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Information level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        protected void LogInfo(string message)
        {
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info(message);
            }
        }

        /// <summary>
        /// Information level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        /// <param name="e">The exception that needs to be logged.</param>
        protected void LogInfo(string message, Exception e)
        {
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info(message, e);
            }
        }

        /// <summary>
        /// Warning level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        protected void LogWarn(string message)
        {
            if (this.logger.IsWarnEnabled)
            {
                this.logger.Warn(message);
            }
        }

        /// <summary>
        /// Warning level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        /// <param name="e">The exception that needs to be logged.</param>
        protected void LogWarn(string message, Exception e)
        {
            if (this.logger.IsWarnEnabled)
            {
                this.logger.Warn(message, e);
            }
        }

        /// <summary>
        /// Error level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        protected void LogError(string message)
        {
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(message);
            }
        }

        /// <summary>
        /// Error level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged.</param>
        /// <param name="e">The exception that needs to be logged.</param>
        protected void LogError(string message, Exception e)
        {
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(message, e);
            }
        }

        /// <summary>
        /// Debug level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged</param>
        protected void LogDebug(string message)
        {
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug(message);
            }
        }

        /// <summary>
        /// Debug level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged</param>
        /// <param name="e">The exception that needs to be logged</param>
        protected void LogDebug(string message, Exception e)
        {
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug(message, e);
            }
        }

        /// <summary>
        /// Fatal level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged</param>
        protected void LogFatal(string message)
        {
            if (this.logger.IsFatalEnabled)
            {
                this.logger.Fatal(message);
            }
        }

        /// <summary>
        /// Fatal level messages are logged to the logger.
        /// </summary>
        /// <param name="message">String that needs to be logged</param>
        /// <param name="e">The exception that needs to be logged</param>
        protected void LogFatal(string message, Exception e)
        {
            if (this.logger.IsFatalEnabled)
            {
                this.logger.Fatal(message, e);
            }
        }

        #endregion

        #endregion
        #endregion

        private void LogEntityInfo(E entity)
        {
            PropertyInfo[] allEntityProperties = entity.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in allEntityProperties)
            {
                string logItem = new StringBuilder().Append(propertyInfo.Name)
                    .Append(" [type = ").Append(propertyInfo.PropertyType)
                    .Append("] [value = ")
                    .Append(propertyInfo.GetValue(entity, null))
                    .Append("]").ToString();
                this.LogInfo(logItem);
            }
        }

        #region IDisposable implementation

        private bool disposedValue;
        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected new void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state here if required             }            
                    // dispose unmanaged objects and set large fields to null     
                    if (_context != null)
                        _context.Dispose();
                } this.disposedValue = true;
            }
        }
        /// <summary>
        /// Dispose method
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
