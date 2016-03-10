using System;

namespace MvcStuff
{
    /// <summary>
    /// Represents a disposer that passes data to the disposition delegate.
    /// </summary>
    /// <typeparam name="T">Type of the data that is passed to the disposer delegate.</typeparam>
    public class DataDisposer<T> : DataDisposerBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataDisposer{T}"/> class.
        /// </summary>
        /// <param name="disposeAction">
        /// The disposition delegate that will be called when disposing the object.
        /// </param>
        public DataDisposer(Action<T> disposeAction)
            : base(disposeAction)
        {
        }

        /// <summary>
        /// Gets or sets the data to be passed to the disposition delegate.
        /// </summary>
        public T Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
    }
}
