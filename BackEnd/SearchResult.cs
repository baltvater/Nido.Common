using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// Base Search request object.
    /// This will be used to pass search requests from UI layer to the Bll
    /// </summary>
    public abstract class SearchRequest
    {
        /// <summary>
        /// The list of search criterias.
        /// This can hold multiple search criterias.
        /// </summary>
        public List<SearchItem> SearchItemList { get; set; }
    }

    /// <summary>
    /// Generic search result, which has the option of passin the search results back to the UI.
    /// This will not only have the result but the search criterias.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchResult<T> : SearchRequest
        where T : BaseObject
    {   
        /// <summary>
        /// List of resulting objects. 
        /// As the search result define this get assign to the correct type.
        /// </summary>
        public List<T> ResultingList { get; set; }
    }
}
