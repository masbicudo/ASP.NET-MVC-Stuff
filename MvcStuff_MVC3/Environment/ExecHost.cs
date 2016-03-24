namespace MvcStuff
{
    /// <summary>
    /// Enumeration of known execution host.
    /// </summary>
    public enum ExecHost
    {
        /// <summary>
        /// Unknown host environment.
        /// </summary>
        Unknown,

        /// <summary>
        /// IIS full version (not express, not embedded).
        /// Azure runs websites in this process, even when it's a Web Role.
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

        /// <summary>
        /// Web Role execution host. Note that Azure websites don't run in this process.
        /// </summary>
        WindowsAzureWebRoleHost,

        /// <summary>
        /// Worker Role execution host.
        /// </summary>
        WindowsAzureWorkerRoleHost,
    }
}