using System;
using System.Diagnostics;
using System.Web.Hosting;

namespace MvcStuff
{
    /// <summary>
    /// Helps getting information that could vary between DEBUG and RELEASE modes.
    /// </summary>
    public class DebugInfo
    {
        private static bool? tempIsDebug;

        /// <summary>
        /// Useful to avoid C# and Resharper complaining about unreachable code,
        /// after #if DEBUG #endif statements, and about unused values before the #if DEBUG code.
        /// </summary>
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                lock (locker)
                    return tempIsDebug ?? true;
#endif
                // pragma: disable warning about unreachable code
#pragma warning disable 162
                // ReSharper disable HeuristicUnreachableCode
                return false;
                // ReSharper restore HeuristicUnreachableCode
#pragma warning restore 162
            }
        }

        /// <summary>
        /// Sets the IsDebug flag to return false for the duration of the returned disposable object.
        /// This does not work when compiling in RELEASE mode.
        /// </summary>
        /// <returns>Disposable object that returns the IsDebug flag to the original state.</returns>
        public static Disposer SetDebug(bool value)
        {
            lock (locker)
                tempIsDebug = value;

            return new Disposer(
                () =>
                {
                    lock (locker)
                        tempIsDebug = null;
                });
        }

        private static readonly object locker = new object();

        public static bool NoHttps
        {
            get { return false; }
        }
    }
}