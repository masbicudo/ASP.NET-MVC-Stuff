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
        /// <summary>
        /// Gets whether the DEBUG directive is defined.
        /// </summary>
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
        
        private static readonly object locker = new object();

        private static HostingServer? _server;

        /// <summary>
        /// Gets the current running host environment.
        /// </summary>
        public static HostingServer HostEnvironment
        {
            get
            {
                if (!_server.HasValue)
                    lock (locker)
                        if (!_server.HasValue)
                        {
                            var isBuilding = HostingEnvironment.InClientBuildManager;

                            //var isdev = HostingEnvironment.IsDevelopmentEnvironment;

                            var ishosted = HostingEnvironment.IsHosted;

                            var procName = Process.GetCurrentProcess().ProcessName;
                            if (String.Equals(procName, "iisexpress"))
                            {
                                _server = HostingServer.IisExpress;
                            }
                            else if (String.Equals(procName, "w3wp"))
                            {
                                _server = HostingServer.Iis;
                            }
                            else
                            {
                                _server = HostingServer.Unknown;
                            }
                        }

                return _server.Value;
            }
        }

        public static bool NoHttps
        {
            get { return false; }
        }
    }

    public enum HostingServer
    {
        /// <summary>
        /// Unknown host environment.
        /// </summary>
        Unknown,

        /// <summary>
        /// IIS full version (not express, not embedded).
        /// </summary>
        Iis,

        /// <summary>
        /// IIS Express.
        /// </summary>
        IisExpress,

        /// <summary>
        /// Visual Studio web development server.
        /// </summary>
        WebDevServer,
    }
}