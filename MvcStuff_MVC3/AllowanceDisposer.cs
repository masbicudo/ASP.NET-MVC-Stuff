using System;

namespace MvcStuff
{
    /// <summary>
    /// Represents a disposer that can be allowed or denied.
    /// </summary>
    public class AllowanceDisposer : DataDisposerBase<bool?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllowanceDisposer"/> class.
        /// </summary>
        /// <param name="disposeAction">
        /// The disposition delegate that will be called when disposing the object.
        /// </param>
        public AllowanceDisposer(Action<bool?> disposeAction)
            : base(disposeAction)
        {
        }

        /// <summary>
        /// Sets the allowance flag to true.
        /// </summary>
        public void Allow()
        {
            this.data = true;
        }

        /// <summary>
        /// Sets the allowance flag to false.
        /// </summary>
        public void Deny()
        {
            this.data = false;
        }
    }
}