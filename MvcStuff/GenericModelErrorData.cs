using System;

namespace MvcStuff
{
    /// <summary>
    /// Format agnostic model error.
    /// </summary>
    [Serializable]
    public class GenericModelErrorData
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Gets or sets the names of the fields or parameters that contributed to this error.
        /// </summary>
        public string[] Names { get; set; }
    }
}