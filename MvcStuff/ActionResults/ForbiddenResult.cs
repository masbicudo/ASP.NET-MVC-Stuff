using System.ComponentModel;
using System.Net;

namespace MvcStuff
{
    /// <summary>
    /// Representa um resultado contendo um código de status 403 (Forbidden).
    /// Esse resultado pode ser usando tanto para requisições normais,
    /// como para requisições JSON ou XML (Ajax).
    /// </summary>
    public class ForbiddenResult : StatusCodeResult
    {
        public ForbiddenResult()
            : this(null)
        {
        }

        public ForbiddenResult([Localizable(true)] string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    }
}