using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common.BackEnd;

namespace Nido.Common.FrontEnd
{
    /// <summary>
    /// Control modes types
    /// </summary>
    public enum ControlModes
    {
        /// <summary>
        /// User is in new mode
        /// </summary>
        New,
        /// <summary>
        /// user is in edit mode
        /// </summary>
        Edit,
        /// <summary>
        /// user is in view mode
        /// </summary>
        View
    }

    /// <summary>
    /// Implement all the WebUserControls using this class
    /// </summary>
    /// <typeparam name="T">The entity object that primarily binded with the UserControl</typeparam>
    public abstract class BaseUserControl<T> : System.Web.UI.UserControl
        where T : class
    {
        /// <summary>
        /// The binded object of the user control, if any.
        /// </summary>
        public abstract T BindedObject { get; set; }
        /// <summary>
        /// The default method to reset data of the user control
        /// </summary>
        protected abstract void ResetData();

        protected virtual void LoadData() { }
        /// <summary>
        /// Control mode of the user control. 
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
        /// If edit = enable editting the user control.
        /// View = disable editting and show only the data in view mode. 
        /// Delete = Allow deleting the items from the user control.
        /// </summary>
        /// <param name="ControlMode">control mode to set the user control</param>
        public abstract void UpdateUI(ControlModes ControlMode);
    }
}
