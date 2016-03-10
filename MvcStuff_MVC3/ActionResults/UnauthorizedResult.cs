using System.ComponentModel;
using System.Net;

namespace MvcStuff
{
    /// <summary>
    /// Representa um resultado contendo um código de status 401 (Unauthorized).
    /// Esse resultado pode ser usando tanto para requisições normais,
    /// como para requisições JSON ou XML (Ajax).
    /// </summary>
    public class UnauthorizedResult : StatusCodeResult
    {
        public UnauthorizedResult()
            : this(null)
        {
        }

        public UnauthorizedResult([Localizable(true)] string statusDescription)
            : base(HttpStatusCode.Unauthorized, statusDescription)
        {
        }
    }
}