using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common.BackEnd;
namespace Nido.Common.FrontEnd
{
    /// <summary>
    /// Base page object. 
    /// All the web pages of the project needs to derive from this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PageBase<T> : System.Web.UI.Page
        where T : BaseObject
    {
        /// <summary>
        /// The binded object of the page, if any.
        /// </summary>
        public abstract T BindedObject { get; set; }
        /// <summary>
        /// The default method to reset data of the page
        /// </summary>
        protected abstract void ResetData();
        /// <summary>
        /// Control mode of the page. 
        /// This indicate whether it is editting, viewing or deleting.
        /// </summary>
        public virtual ControlModes ControlMode
        {
            get
            {
                return SessionHelper.GetValue<ControlModes>(this.UniqueID);
            }
            set
            {
                SessionHelper.SetValue(this.UniqueID, value);
                UpdateUI(ControlMode);
            }
        }
        /// <summary>
        /// This change the UI to the corresponding control mode.
        /// If edit = enable editting the page.
        /// View = disable editting and show only the data in view mode. 
        /// Delete = Allow deleting the items from the page.
        /// </summary>
        /// <param name="ControlMode">control mode to set the page</param>
        public abstract void UpdateUI(ControlModes ControlMode);
    }
}
