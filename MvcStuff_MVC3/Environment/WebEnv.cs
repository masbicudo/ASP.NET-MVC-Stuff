using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;

namespace MvcStuff
{
    /// <summary>
    /// Contains information about the environment that are usefull to web applications.
    /// </summary>
    public static class WebEnv
    {
        private static readonly object locker = new object();

        private static bool? _isDev;

        public static bool IsDevelopment
        {
            get
            {
                if (!_isDev.HasValue)
                    lock (locker)
                        if (!_isDev.HasValue)
                        {
                            var value = GetIsDevelopment();
                            Thread.MemoryBarrier();
                            _isDev = value;
                        }

                return _isDev.Value;
            }
        }

        private static bool GetIsDevelopment()
        {
#if ASP_MVC_3 || ASP_MVC_4
            return GetWasLaunchedFromDevelopmentEnvironmentValue();
#else
            return System.Web.Hosting.HostingEnvironment.IsDevelopmentEnvironment;
#endif
        }

#if ASP_MVC_3 || ASP_MVC_4
        private static bool GetWasLaunchedFromDevelopmentEnvironmentValue()
        {
            try
            {
                var devEnvironment = Environment.GetEnvironmentVariable("DEV_ENVIRONMENT", EnvironmentVariableTarget.Process);
                return string.Equals(devEnvironment, "1", StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }
#endif

        private static ExecHost? _execHost;

        /// <summary>
        /// Gets a value indicating the execution host of the currently running application.
        /// </summary>
        public static ExecHost ExecutionHost
        {
            get
            {
                if (!_execHost.HasValue)
                    lock (locker)
                        if (!_execHost.HasValue)
                        {
                            var value = GetExecutionHost();
                            Thread.MemoryBarrier();
                            _execHost = value;
                        }

                return _execHost.Value;
            }
        }

        private static ExecHost GetExecutionHost()
        {
            var exeName = Process.ProcessName;

            if (!string.IsNullOrWhiteSpace(exeName))
            {
                if (string.Equals(exeName, "iisexpress", StringComparison.InvariantCultureIgnoreCase))
                    return ExecHost.IisExpress;

                if (string.Equals(exeName, "w3wp", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ExecHost.Iis;
                }

                if (Regex.IsMatch(exeName, @"^WebDev.WebServer\d*$", RegexOptions.IgnoreCase))
                    return ExecHost.WebDevServer;

                // Web roles are hosted by two processes (WaIISHost.exe and w3wp.exe)
                if (string.Equals(exeName, "WaIISHost", StringComparison.InvariantCultureIgnoreCase))
                    return ExecHost.WindowsAzureWebRoleHost;

                // Worker roles are hosted by a single process (WaWorkerHost.exe)
                if (string.Equals(exeName, "WaWorkerHost", StringComparison.InvariantCultureIgnoreCase))
                    return ExecHost.WindowsAzureWorkerRoleHost;
            }

            return ExecHost.Unknown;
        }

        private static HostEnv? _hostEnv;

        /// <summary>
        /// Gets a value indicating the environment of the currently running application.
        /// </summary>
        public static HostEnv HostEnvironment
        {
            get
            {
                if (!_hostEnv.HasValue)
                    lock (locker)
                        if (!_hostEnv.HasValue)
                        {
                            var value = GetHostEnvironment();
                            Thread.MemoryBarrier();
                            _hostEnv = value;
                        }

                return _hostEnv.Value;
            }
        }

        private static HostEnv GetHostEnvironment()
        {
            var execHost = ExecutionHost;

            if (execHost == ExecHost.Iis)
            {
                if (AzureWebSite.HostName.EndsWith(".azurewebsites.net"))
                    return HostEnv.WindowsAzureWebSite;

                return HostEnv.Iis;
            }

            if (execHost == ExecHost.WebDevServer)
                return HostEnv.WebDevServer;

            if (execHost == ExecHost.IisExpress)
                return HostEnv.IisExpress;

            if (!string.IsNullOrWhiteSpace(AzureWebJob.Name))
                return HostEnv.WindowsAzureWebJob;

            return HostEnv.Unknown;
        }

        private static Process _process;

        /// <summary>
        /// Gets the currently running process.
        /// </summary>
        public static Process Process
        {
            get
            {
                if (_process == null)
                    lock (locker)
                        if (_process == null)
                        {
                            var value = Process.GetCurrentProcess();
                            Thread.MemoryBarrier();
                            _process = value;
                        }

                return _process;
            }
        }

        public static class AzureWebSite
        {
            // https://github.com/projectkudu/kudu/wiki/Azure-runtime-environment

            /// <summary>
            /// Gets the name of the site.
            /// </summary>
            public static string SiteName { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");

            /// <summary>
            /// Gets the sku of the site (Possible values: Free, Shared, Basic, Standard).
            /// </summary>
            public static string Sku { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_SKU");

            /// <summary>
            /// Gets a value that specifies whether website is on a dedicated or shared VM/s (Possible values: Shared, Dedicated).
            /// </summary>
            public static string ComputeMode { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_COMPUTE_MODE");

            /// <summary>
            /// Gets the mode for the site (can be Limited for a free site, Basic for a shared site or empty for a standard site).
            /// </summary>
            public static string SiteMode { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_MODE");

            /// <summary>
            /// Gets the Azure Website's primary host name for the site (For example: site.azurewebsites.net). Note that custom hostnames are not accounted for here.
            /// </summary>
            public static string HostName { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

            /// <summary>
            /// Gets the id representing the VM that the site is running on (If site runs on multiple instances, each instance will have a different id).
            /// </summary>
            public static string InstanceId { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");

            /// <summary>
            /// Gets the default node version this website is using.
            /// </summary>
            public static string NodeDefaultVersion { get; } = System.Environment.GetEnvironmentVariable("WEBSITE_NODE_DEFAULT_VERSION");

            /// <summary>
            /// Gets the limit for websocket's concurrent requests.
            /// </summary>
            public static string WebsocketConcurrentRequestLimit { get; } = System.Environment.GetEnvironmentVariable("WEBSOCKET_CONCURRENT_REQUEST_LIMIT");
        }

        public static class AzureWebJob
        {
            // https://github.com/projectkudu/kudu/wiki/Web-Jobs#environment-settings

            /// <summary>
            /// Gets the root path of currently running job (will be under some temporary directory).
            /// </summary>
            public static string Path { get; } = System.Environment.GetEnvironmentVariable("WEBJOBS_PATH");

            /// <summary>
            /// Gets the current job name.
            /// </summary>
            public static string Name { get; } = System.Environment.GetEnvironmentVariable("WEBJOBS_NAME");

            /// <summary>
            /// Gets the current job type (triggered/continuous).
            /// </summary>
            public static string Type { get; } = System.Environment.GetEnvironmentVariable("WEBJOBS_TYPE");

            /// <summary>
            /// Gets the current job meta data path, this includes the job's logs/history and any artifact of the job can go there.
            /// </summary>
            public static string DataPath { get; } = System.Environment.GetEnvironmentVariable("WEBJOBS_DATA_PATH");

            /// <summary>
            /// Gets the current run id of the job (used for triggered jobs).
            /// </summary>
            public static string RunId { get; } = System.Environment.GetEnvironmentVariable("WEBJOBS_RUN_ID");
        }
    }
}