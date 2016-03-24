namespace MvcStuff
{
    /// <summary>
    /// Host environment of the current application.
    /// </summary>
    public enum HostEnv
    {
        /// <summary>
        /// Unknown host environment.
        /// </summary>
        Unknown,

        /// <summary>
        /// IIS full version environment (not express, not embedded).
        /// </summary>
        Iis,

        /// <summary>
        /// IIS Express environment.
        /// </summary>
        IisExpress,

        /// <summary>
        /// Visual Studio web development server environment.
        /// </summary>
        WebDevServer,

        /// <summary>
        /// Web Role environment.
        /// </summary>
        WindowsAzureWebRole,

        /// <summary>
        /// Worker Role environment.
        /// </summary>
        WindowsAzureWorkerRole,

        WindowsAzureWebSite,

        WindowsAzureWebJob
    }
}