using System;

namespace MvcStuff
{
    /// <summary>
    /// Generic disposer that can pass data to the disposition delegate.
    /// </summary>
    /// <typeparam name="T">Type of the data that is passed to the disposer delegate.</typeparam>
    public class DataDisposerBase<T> : IDisposable
    {
        private readonly Action<T> disposeAction;

        protected T data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDisposerBase{T}"/> class.
        /// </summary>
        /// <param name="disposeAction">
        /// The disposition delegate that will be called when disposing the object.
        /// </param>
        public DataDisposerBase(Action<T> disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        /// <summary>
        /// Disposes this object passing the disposition data to the disposition delegate.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeAction != null)
            {
                this.disposeAction(this.data);
            }
        }
    }
}