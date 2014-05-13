using System.ComponentModel;
using System.Net;

namespace MvcStuff
{
    /// <summary>
    /// Representa um resultado contendo um código de status 404 (NotFound).
    /// Esse resultado pode ser usando tanto para requisições normais,
    /// como para requisições JSON ou XML (Ajax).
    /// </summary>
    public class NotFoundResult : StatusCodeResult
    {
        public NotFoundResult()
            : this(null)
        {
        }

        public NotFoundResult([Localizable(true)] string statusDescription)
            : base(HttpStatusCode.NotFound, statusDescription)
        {
        }
    }
}