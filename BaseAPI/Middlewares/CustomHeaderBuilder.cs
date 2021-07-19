using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Middlewares
{
  
    public class CustomHeaderBuilder
    {
        private readonly CustomHeadersPolicy _policy = new CustomHeadersPolicy();

        /// <summary>
        /// The number of seconds in one year
        /// </summary>
        public const int OneYearInSeconds = 60 * 60 * 24 * 365;

        /// <summary>
        /// Add default headers in accordance with most secure approach
        /// </summary>
        public CustomHeaderBuilder AddDefaultSecurePolicy()
        {
            AddCorrelationID();
            return this;
        }

        public CustomHeaderBuilder AddCorrelationID()
        {
            _policy.SetHeaders["CorrelationId"] = Guid.NewGuid().ToString();
            return this;
        }

        /// <summary>
        /// Adds a custom header to all requests
        /// </summary>
        /// <param name="header">The header name</param>
        /// <param name="value">The value for the header</param>
        /// <returns></returns>
        public CustomHeaderBuilder AddCustomHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.SetHeaders[header] = value;
            return this;
        }

        /// <summary>
        /// Remove a header from all requests
        /// </summary>
        /// <param name="header">The to remove</param>
        /// <returns></returns>
        public CustomHeaderBuilder RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.RemoveHeaders.Add(header);
            return this;
        }

        /// <summary>
        /// Builds a new <see cref="CustomHeaderBuilder"/> using the entries added.
        /// </summary>
        /// <returns>The constructed <see cref="CustomHeaderBuilder"/>.</returns>
        public CustomHeadersPolicy Build()
        {
            return _policy;
        }
    }
}
