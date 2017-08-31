using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// Generic base Response Object. 
    /// This is the common object that uses to 
    /// return object from business layer to the UI Layer.
    /// If you do not want a result to be passed to the UI layer you may use this object.
    /// </summary>
    /// <example><code lang="C#">
    /// Bll.Handlers.BankHandler bankHandler = new BankHandler();
    /// RadGrid bankGrid = (RadGrid)ucBank1.FindControl("tGrdBank");
    /// GenericResponse&lt;IEnumerable&lt;Bank>> response = bankHandler.GetAllGeneric();
    /// if (response.Successful)
    /// {
    ///      Response.Write(response.Messages[0]); // Display all the  messages to the user
    /// }
    /// else
    /// {
    ///      Response.Write(response.Messages[0]); // Display all the error messages to the user
    /// }
    /// </code></example>
    public class GenericResponse
    {
        /// <summary>
        /// Default constructor of the generic response object
        /// </summary>
        public GenericResponse()
        {
            Successful = true;
        }

        /// <summary>
        /// user define contructor of the generic response object
        /// </summary>
        /// <param name="successful">send the success or failure status of the response</param>
        /// <param name="message">Status message, to be displayed to the user</param>
        public GenericResponse(bool successful, string message)
        {
            Successful = successful;
            Messages.Add(message);
        }

        /// <summary>
        /// user define contructor of the response object
        /// </summary>
        /// <param name="successful">send the success or failure status of the response</param>
        /// <param name="messages">The List of status message, to be displayed to the user. 
        /// This is needed when multiple operations are done on a single method call.</param>
        public GenericResponse(bool successful, string[] messages)
        {
            Successful = successful;
            Messages.AddRange(messages);
        }

        /// <summary>
        /// List of messages, to be displayed to the user. This gives details about 
        /// what happen to the called request and whether it is success or a failure.
        /// In addition to that this property gives a special ID for user to 
        /// report any error via MAS-IT help desk.
        /// </summary>
        public List<string> Messages = new List<string>();
        /// <summary>
        /// Indicated whether the operation is a success or a failure.
        /// </summary>
        public bool Successful { get; set; }
        /// <summary>
        /// If any error occured this gives the error message of the primary exception.
        /// Level 1 error message
        /// </summary>
        public string Error1 { get; set; }
        /// <summary>
        /// If any error occured this gives the error message of the inner exception.
        /// Message of the inner exception
        /// </summary>
        public string Error2 { get; set; }
        /// <summary>
        /// This give a reference to the actual exception occured. 
        /// This way UI layer will have all the details of the error, 
        /// hence can decide how to handle the error in the best possible manner.
        /// </summary>
        public Exception CurrentException { get; set; }
        /// <summary>
        /// Title of the error.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Generic base Response Object. 
    /// This is the common object that uses to 
    /// return object from business layer to the UI Layer. 
    /// This mode of the response support sending the custom result object.
    /// </summary>
    /// <example><code lang="C#">
    /// Bll.Handlers.BankHandler bankHandler = new BankHandler();
    /// 
    /// RadGrid bankGrid = (RadGrid)ucBank1.FindControl("tGrdBank");
    /// 
    /// GenericResponse&lt;IEnumerable&lt;Nido.Transport.Bll.Models.Bank&gt;&gt; response = bankHandler.GetAllGeneric();
    /// if (response.Successful)
    /// {
    ///      bankGrid.DataSource = response.Result.ToList();
    ///      bankGrid.DataBind();
    ///      ucBank1.ControlMode = Nido.Common.FrontEnd.ControlModes.New;
    /// }
    /// else
    /// {
    ///      Response.Write(response.Messages[0]); // Display all the error messages to the user
    /// }
    /// </code></example>
    public class GenericResponse<TResult> : GenericResponse
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericResponse()
        {
        }

        /// <summary>
        /// Custom generic response object
        /// </summary>
        /// <param name="result"></param>
        /// <param name="successful"></param>
        /// <param name="message"></param>
        public GenericResponse(TResult result, bool successful, string message)
            : base(successful, message)
        {
            Result = result;
        }

        /// <summary>
        /// User defined Generic response object
        /// </summary>
        /// <param name="result">The the resulting object</param>
        /// <param name="successful">Operation status, true of successful and false otherwise</param>
        /// <param name="messages">The messages to be displayed to the user</param>
        public GenericResponse(TResult result, bool successful, string[] messages)
            : base(successful, messages)
        {
            Result = result;
        }

        /// <summary>
        /// User defined Generic response object
        /// </summary>
        /// <param name="successful">Operation status, true of successful and false otherwise</param>
        /// <param name="messages">The messages to be displayed to the user</param>
        public GenericResponse(bool successful, string[] messages)
            : base(successful, messages) { }

        /// <summary>
        /// Result of the calling function.
        /// </summary>
        public TResult Result { get; set; }
    }
}
