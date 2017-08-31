using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// Supportive enum for search item.
    /// This defines the comparision types.
    /// </summary>
    public enum Compareres
    {
        /// <summary>
        /// Indicate the the comparision is equal type
        /// </summary>
        Equal,
        /// <summary>
        /// Indicate the the comparision is like type
        /// </summary>
        Like
    }

    /// <summary>
    /// The search item carries search request details from UI layer to the Bll
    /// </summary>
    public class SearchItem
    {
        /// <summary>
        /// The field to be searched
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// Actual value to be used for searching
        /// </summary>
        public string SearchValue { get; set; }
        /// <summary>
        /// Comparision to compare with the field agains the search value
        /// </summary>
        public Compareres   Comparision { get; set; }
    }
}
