using System.ComponentModel;
using System.Net;

namespace MvcStuff
{
    #if pt_BR
    /// <summary>
    /// Representa um resultado contendo um código de status 404 (NotFound).
    /// Esse resultado pode ser usando tanto para requisições normais,
    /// como para requisições JSON ou XML (Ajax).
    /// </summary>
    #else
    /// <summary>
    /// Represents a result with 404 status code (NotFound).
    /// This result can be used by regular Html requests,
    /// or else by JSON or XML requests (Ajax).
    /// </summary>
    #endif
    public class NotFoundResult : StatusCodeResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundResult"/> class.
        /// </summary>
        public NotFoundResult()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundResult"/> class
        /// along with a user friendly description.
        /// </summary>
        /// <param name="statusDescription"> User friendly status description. </param>
        public NotFoundResult([Localizable(true)] string statusDescription)
            : base(HttpStatusCode.NotFound, statusDescription)
        {
        }
    }
}