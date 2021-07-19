using BaseAPI.Middlewares.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Middlewares
{
    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        /// <summary>
        /// The number of seconds in one year
        /// </summary>
        public const int OneYearInSeconds = 60 * 60 * 24 * 365;

        /// <summary>
        /// Add default headers in accordance with most secure approach
        /// </summary>
        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            AddFrameOptionsDeny();
            //AddXssProtectionBlock();
            AddContentTypeOptionsNoSniff();
            AddStrictTransportSecurityMaxAge();
            RemoveServerHeader();

            return this;
        }

        /// <summary>
        /// Add X-Frame-Options DENY to all requests.
        /// The page cannot be displayed in a frame, regardless of the site attempting to do so
        /// </summary>
        public SecurityHeadersBuilder AddFrameOptionsDeny()
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.Deny;
            return this;
        }

        /// <summary>
        /// Add X-Frame-Options SAMEORIGIN to all requests.
        /// The page can only be displayed in a frame on the same origin as the page itself.
        /// </summary>
        public SecurityHeadersBuilder AddFrameOptionsSameOrigin()
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.SameOrigin;
            return this;
        }

        /// <summary>
        /// Add X-Frame-Options ALLOW-FROM {uri} to all requests, where the uri is provided
        /// The page can only be displayed in a frame on the specified origin.
        /// </summary>
        /// <param name="uri">The uri of the origin in which the page may be displayed in a frame</param>
        public SecurityHeadersBuilder AddFrameOptionsSameOrigin(string uri)
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = string.Format(FrameOptionsConstants.AllowFromUri, uri);
            return this;
        }    

        /// <summary>
        /// Add Strict-Transport-Security max-age=<see cref="maxAge"/> to all requests.
        /// Tells the user-agent to cache the domain in the STS list for the number of seconds provided.
        /// </summary>
        public SecurityHeadersBuilder AddStrictTransportSecurityMaxAge(int maxAge = OneYearInSeconds)
        {
            _policy.SetHeaders[StrictTransportSecurityConstants.Header] =
                string.Format(StrictTransportSecurityConstants.MaxAge, maxAge);
            return this;
        }

        /// <summary>
        /// Add Strict-Transport-Security max-age=<see cref="maxAge"/>; includeSubDomains to all requests.
        /// Tells the user-agent to cache the domain in the STS list for the number of seconds provided and include any sub-domains.
        /// </summary>
        public SecurityHeadersBuilder AddStrictTransportSecurityMaxAgeIncludeSubDomains(int maxAge = OneYearInSeconds)
        {
            _policy.SetHeaders[StrictTransportSecurityConstants.Header] =
                string.Format(StrictTransportSecurityConstants.MaxAgeIncludeSubdomains, maxAge);
            return this;
        }

        /// <summary>
        /// Add Strict-Transport-Security max-age=0 to all requests.
        /// Tells the user-agent to remove, or not cache the host in the STS cache
        /// </summary>
        public SecurityHeadersBuilder AddStrictTransportSecurityNoCache()
        {
            _policy.SetHeaders[StrictTransportSecurityConstants.Header] =
                StrictTransportSecurityConstants.NoCache;
            return this;
        }

        /// <summary>
        /// Add X-Content-Type-Options nosniff to all requests.
        /// Can be set to protect against MIME type confusion attacks.
        /// </summary>
        public SecurityHeadersBuilder AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders[ContentTypeOptionsConstants.Header] = ContentTypeOptionsConstants.NoSniff;
            return this;
        }

        /// <summary>
        /// Removes the Server header from all responses
        /// </summary>
        public SecurityHeadersBuilder RemoveServerHeader()
        {
            _policy.RemoveHeaders.Add(ServerConstants.Header);
            return this;
        }

        /// <summary>
        /// Adds a custom header to all requests
        /// </summary>
        /// <param name="header">The header name</param>
        /// <param name="value">The value for the header</param>
        /// <returns></returns>
        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
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
        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.RemoveHeaders.Add(header);
            return this;
        }

        /// <summary>
        /// Builds a new <see cref="SecurityHeadersPolicy"/> using the entries added.
        /// </summary>
        /// <returns>The constructed <see cref="SecurityHeadersPolicy"/>.</returns>
        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}
