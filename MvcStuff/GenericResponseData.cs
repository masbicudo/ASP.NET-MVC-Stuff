using System;

namespace MvcStuff
{
    /// <summary>
    /// Format agnostic response data.
    /// </summary>
    [Serializable]
    public class GenericResponseData
    {
        /// <summary>
        /// Gets or sets an URL to redirect to.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a success response.
        /// </summary>
        public bool Ok { get; set; }

        /// <summary>
        /// Gets or sets an user friendly text message.
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Gets or sets an error id.
        /// This is generally used when an exception happens.
        /// </summary>
        public string ErrId { get; set; }

        /// <summary>
        /// Gets or sets the status of the current response.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets an array of errors that tells what is wrong with the current request.
        /// </summary>
        public GenericModelErrorData[] Errs { get; set; }

        /// <summary>
        /// Gets or sets a bag of values, containing additional data to the view.
        /// </summary>
        public dynamic Bag { get; set; }
    }
}
