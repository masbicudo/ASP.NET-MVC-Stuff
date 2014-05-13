using System.Web.Mvc;

namespace MvcStuff
{
    /// <summary>
    /// Extended view page is a WebViewPage with more things to help in the view.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// It also enables using views to change values in the ViewData dictionary,
    /// so that a caller can see changed values. This is useful when rendering
    /// views manually to a string for example, and you want to know the title
    /// that the view used inside of it.
    /// </remarks>
    public abstract class ExtendedViewPage<TModel> : WebViewPage<TModel>
    {
        private ViewDataDictionary originalViewData;

        /// <summary>
        /// Sets the view data.
        /// </summary>
        /// <param name="viewData">The view data.</param>
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            this.originalViewData = viewData;
            base.SetViewData(viewData);
        }

        /// <summary>
        /// Runs the page hierarchy for the ASP.NET Razor execution pipeline.
        /// </summary>
        public override void ExecutePageHierarchy()
        {
            if (DebugInfo.IsDebug)
                this.WriteLiteral(string.Format("<!--BEGIN VIEW: {0}-->", this.VirtualPath));

            base.ExecutePageHierarchy();

            if (DebugInfo.IsDebug)
                this.WriteLiteral(string.Format("<!--END VIEW: {0}-->", this.VirtualPath));

            // after executing the page, we need to copy value from the cloned ViewData
            // to the original ViewData... so that the caller can see elements inserted
            // or changed by the view
            foreach (var eachViewDataItem in this.ViewData)
                this.originalViewData[eachViewDataItem.Key] = eachViewDataItem.Value;
        }
    }

    /// <summary>
    /// Extended view page is a WebViewPage with more things to help in the view.
    /// </summary>
    public abstract class ExtendedViewPage : WebViewPage
    {
        private ViewDataDictionary originalViewData;

        /// <summary>
        /// Sets the view data.
        /// </summary>
        /// <param name="viewData">The view data.</param>
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            this.originalViewData = viewData;
            base.SetViewData(viewData);
        }

        /// <summary>
        /// Runs the page hierarchy for the ASP.NET Razor execution pipeline.
        /// </summary>
        public override void ExecutePageHierarchy()
        {
            if (DebugInfo.IsDebug)
                this.WriteLiteral(string.Format("<!--BEGIN VIEW: {0}-->", this.VirtualPath));

            base.ExecutePageHierarchy();

            if (DebugInfo.IsDebug)
                this.WriteLiteral(string.Format("<!--END VIEW: {0}-->", this.VirtualPath));

            // after executing the page, we need to copy value from the cloned ViewData
            // to the original ViewData... so that the caller can see elements inserted
            // or changed by the view
            foreach (var eachViewDataItem in this.ViewData)
                this.originalViewData[eachViewDataItem.Key] = eachViewDataItem.Value;
        }
    }
}
