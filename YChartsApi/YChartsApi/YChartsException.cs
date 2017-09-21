using System;

namespace YCharts.Api
{
    /// <summary> The base class of all YCharts API exceptions</summary>
    public sealed class YChartsException : Exception
    {
        /// <summary> Corresponds to a request HTTP status code</summary>
        public int StatusCode;

        /// <summary> Constructs an ApiException with both a status code and reason</summary>
        /// <param name="message"> Reason for exception</param>
        /// <param name="statusCode">HTTP status code of exception</param>
        internal YChartsException(string message, int statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
        /// <summary>
        /// Constructs an ApiException with a reason
        /// </summary>
        /// <param name="message">Reason for exception</param>
        internal YChartsException(string message) : base(message){}

    }
}
