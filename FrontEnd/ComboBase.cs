using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common.BackEnd;
using System.Data.Entity;
using System.Collections;
using System.Web.UI.WebControls;

namespace Nido.Common.FrontEnd
{
    /// <summary>
    /// Use this class to implement Custom Combo Box that directly load Master records.
    /// </summary>
    /// <typeparam name="E">Entity Object</typeparam>
    /// <typeparam name="C">The Custom Database context that you are using</typeparam>
    /// <example>
    /// <code lang="C#">
    /// public class BankDropdown : BaseCombo&lt;Bank, TransportDBContext>
    /// {
    ///     public BankDropdown()
    ///         : base(new BankHandler())
    ///     {
    ///    
    ///     }
    /// }
    /// </code></example>
    public class BaseCombo<E, C> : DropDownList
        where E : BaseObject, new()
        where C : BaseObjectConext, new()
    {
        /// <summary>
        /// generic base handle variable. This is only for local use
        /// </summary>
        /// <returns>
        /// Need to pass via the user define contractor by the inherited class.
        /// </returns>
        /// <typeparam name="E">BaseObject, you can pass the custom business object you have
        /// created in place of this.</typeparam>
        /// <typeparam name="C">DBContext object. you can pass the Context object that you
        /// create in place of this.</typeparam>
        private HandlerBase<E, C> myHandler;
        /// <summary>
        /// User define contructor of the base combo abstract class
        /// </summary>
        /// <param name="handler">Handler object. This is to be used to load the required data list from the database</param>
        public BaseCombo(HandlerBase<E, C> handler)
        {
            myHandler = handler;
        }

        /// <summary>
        /// overridable data binding method.
        /// </summary>
        protected virtual void NidoDataBinding()
        {
            this.Items.Clear();
            GenericResponse<IEnumerable<E>> response = myHandler.GetAllGeneric();
            if (response.Successful)
            {
                IEnumerable<E> list = response.Result;

                //if (list.Count() > 0)
                //{
                    foreach (E item in list)
                    {
                        //this.Items.Add(new ListItem(item.Text, item.Value));
                        this.Items.Add(new ListItem(item.Text, item.Value));
                    }
                    this.SelectedValue = this.Items[0].Value;
                //}
            }
            else
                throw response.CurrentException;
        }

        /// <summary>
        /// On PerformDataBinding method. This is from where system calls to 'NidoDataBinding'
        /// </summary>
        /// <param name="e"></param>
        protected override void PerformDataBinding(IEnumerable retrievedData)
        {
            NidoDataBinding();
        }

    }
}
