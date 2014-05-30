using System;

namespace MvcStuff
{
    /// <summary>
    /// Represents a generic disposer that calls a disposition delegate when disposing.
    /// </summary>
    public class Disposer : IDisposable
    {
        private readonly Action disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer"/> class.
        /// </summary>
        /// <param name="disposeAction">
        /// The disposition delegate that will be called when disposing the object.
        /// </param>
        public Disposer(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        /// <summary>
        /// Disposes this object calling the disposition delegate.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeAction != null)
            {
                this.disposeAction();
            }
        }
    }
}