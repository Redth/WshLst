// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Microsoft.WindowsAzure.MobileServices
{
    /// <summary>
    /// Provides basic access to Mobile Services.
    /// </summary>
    public sealed partial class MobileServiceClient
    {
        /// <summary>
        /// Name of the  JSON member in the config setting that stores the
        /// authentication token.
        /// </summary>
        private const string LoginAsyncAuthenticationTokenKey = "authenticationToken";

        /// <summary>
        /// Relative URI fragment of the login endpoint.
        /// </summary>
        private const string LoginAsyncUriFragment = "login";

        /// <summary>
        /// Relative URI fragment of the login/done endpoint.
        /// </summary>
        private const string LoginAsyncDoneUriFragment = "login/done";

        /// <summary>
        /// Name of the Installation ID header included on each request.
        /// </summary>
        private const string RequestInstallationIdHeader = "X-ZUMO-INSTALLATION-ID";

        /// <summary>
        /// Name of the application key header included when there's a key.
        /// </summary>
        private const string RequestApplicationKeyHeader = "X-ZUMO-APPLICATION";

        /// <summary>
        /// Name of the authentication header included when the user's logged
        /// in.
        /// </summary>
        private const string RequestAuthenticationHeader = "X-ZUMO-AUTH";

        /// <summary>
        /// Content type for request bodies and accepted responses.
        /// </summary>
        private const string RequestJsonContentType = "application/json";

        /// <summary>
        /// The ID used to identify this installation of the application to
        /// provide telemetry data.
        /// </summary>
        private static readonly string applicationInstallationId = MobileServiceApplication.InstallationId;

        /// <summary>
        /// A JWT token representing the current user's successful OAUTH
        /// authorization.
        /// </summary>
        /// <remarks>
        /// This is passed on every request (when it exists) as the X-ZUMO-AUTH
        /// header.
        /// </remarks>
        private string currentUserAuthenticationToken = null;

        /// <summary>
        /// Represents a filter used to process HTTP requests and responses
        /// made by the Mobile Service.  This can only be set by calling
        /// WithFilter to create a new MobileServiceClient with the filter
        /// applied.
        /// </summary>
        private IServiceFilter filter = null;

        /// <summary>
        /// Indicates whether a login operation is currently in progress.
        /// </summary>
        public bool LoginInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the MobileServiceClient class.
        /// </summary>
        /// <param name="applicationUri">
        /// The Uri to the Mobile Services application.
        /// </param>
        public MobileServiceClient(Uri applicationUri)
            : this(applicationUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MobileServiceClient class.
        /// </summary>
        /// <param name="applicationUri">
        /// The Uri to the Mobile Services application.
        /// </param>
        /// <param name="applicationKey">
        /// The application name for the Mobile Services application.
        /// </param>
        public MobileServiceClient(Uri applicationUri, string applicationKey)
        {
            if (applicationUri == null)
            {
                throw new ArgumentNullException("applicationUri");
            }

            this.ApplicationUri = applicationUri;
            this.ApplicationKey = applicationKey;
        }

        /// <summary>
        /// Initializes a new instance of the MobileServiceClient class based
        /// on an existing instance.
        /// </summary>
        /// <param name="service">
        /// An existing instance of the MobileServices class to copy.
        /// </param>
        private MobileServiceClient(MobileServiceClient service)
        {
            Debug.Assert(service != null, "service cannot be null!");

            this.ApplicationUri = service.ApplicationUri;
            this.ApplicationKey = service.ApplicationKey;
            this.CurrentUser = service.CurrentUser;
            this.currentUserAuthenticationToken = service.currentUserAuthenticationToken;
            this.filter = service.filter;
        }

        /// <summary>
        /// Gets the Uri to the Mobile Services application that is provided by
        /// the call to MobileServiceClient(...).
        /// </summary>
        public Uri ApplicationUri { get; private set; }

        /// <summary>
        /// Gets the Mobile Services application's name that is provided by the
        /// call to MobileServiceClient(...).
        /// </summary>
        public string ApplicationKey { get; private set; }

        /// <summary>
        /// The current authenticated user provided after a successful call to
        /// MobileServiceClient.Login().
        /// </summary>
        public MobileServiceUser CurrentUser { get; private set; }

        /// <summary>
        /// Gets a reference to a table and its data operations.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A reference to the table.</returns>
        public IMobileServiceTable GetTable(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException("tableName");
            }
            else if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.EmptyArgumentExceptionMessage,
                        "tableName"));
            }

            return new MobileServiceTable(tableName, this);
        }

        /// <summary>
        /// Create a new MobileServiceClient with a filter used to process all
        /// of its HTTP requests and responses.
        /// </summary>
        /// <param name="filter">The filter to use on the service.</param>
        /// <returns>
        /// A new MobileServiceClient whose HTTP requests and responses will be
        /// filtered as desired.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "filter", Justification = "Consequence of StyleCop enforced naming conventions")]
        public MobileServiceClient WithFilter(IServiceFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            // "Clone" the current service
            MobileServiceClient extended = new MobileServiceClient(this);

            // Chain this service's filter with the new filter for the
            // extended service.  Note that we're 
            extended.filter = (this.filter != null) ?
                new ComposedServiceFilter(this.filter, filter) :
                filter;

            return extended;
        }

        /// <summary>
        /// Log a user into a Mobile Services application given an access
        /// token.
        /// </summary>
        /// <param name="authenticationToken">
        /// OAuth access token that authenticates the user.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        internal Task<MobileServiceUser> SendLoginAsync(string authenticationToken)
        {
            if (authenticationToken == null)
            {
                throw new ArgumentNullException("authenticationToken");
            }
            else if (string.IsNullOrEmpty(authenticationToken))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.EmptyArgumentExceptionMessage,
                        "authenticationToken"));
            }

            // TODO: Decide what we should do when CurrentUser isn't null
            // (i.e., do we just log out the current user or should we throw
            // an exception?).  For now we just overwrite the the current user
            // and their token on a successful login.

            JsonObject request = new JsonObject()
                .Set(LoginAsyncAuthenticationTokenKey, authenticationToken);
            
			return this.RequestAsync("POST", LoginAsyncUriFragment, request)
				.ContinueWith (t =>
				{
					// Get the Mobile Services auth token and user data
					this.currentUserAuthenticationToken = t.Result.Get(LoginAsyncAuthenticationTokenKey).AsString();
					this.CurrentUser = new MobileServiceUser(t.Result.Get("user").Get("userId").AsString());

					return this.CurrentUser;
				});
        }

        /// <summary>
        /// Log a user out of a Mobile Services application.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Logout is preferred by design")]
        public void Logout()
        {
            this.CurrentUser = null;
            this.currentUserAuthenticationToken = null;
        }

        /// <summary>
        /// Perform a web request and include the standard Mobile Services
        /// headers.
        /// </summary>
        /// <param name="method">
        /// The HTTP method used to request the resource.
        /// </param>
        /// <param name="uriFragment">
        /// URI of the resource to request (relative to the Mobile Services
        /// runtime).
        /// </param>
        /// <param name="content">
        /// Optional content to send to the resource.
        /// </param>
        /// <returns>The JSON value of the response.</returns>
        internal Task<IJsonValue> RequestAsync(string method, string uriFragment, IJsonValue content)
        {
            Debug.Assert(!string.IsNullOrEmpty(method), "method cannot be null or empty!");
            Debug.Assert(!string.IsNullOrEmpty(uriFragment), "uriFragment cannot be null or empty!");

            // Create the web request
            IServiceFilterRequest request = new ServiceFilterRequest();
            request.Uri = new Uri(this.ApplicationUri, uriFragment);
            request.Method = method.ToUpper();
            request.Accept = RequestJsonContentType;

            // Set Mobile Services authentication, application, and telemetry
            // headers
            request.Headers[RequestInstallationIdHeader] = applicationInstallationId;
            if (!string.IsNullOrEmpty(this.ApplicationKey))
            {
                request.Headers[RequestApplicationKeyHeader] = this.ApplicationKey;
            }
            if (!string.IsNullOrEmpty(this.currentUserAuthenticationToken))
            {
                request.Headers[RequestAuthenticationHeader] = this.currentUserAuthenticationToken;
            }

            // Add any request as JSON
            if (content != null)
            {
                request.ContentType = RequestJsonContentType;
                request.Content = content.Stringify();
            }

			// Send the request and get the response back as JSON
	        return ServiceFilter.ApplyAsync (request, this.filter)
				.ContinueWith (t =>
				{
					var response = t.Result;
					IJsonValue body = GetResponseJsonAsync (response);

					// Throw errors for any failing responses
					if (response.ResponseStatus != ServiceFilterResponseStatus.Success || response.StatusCode >= 400)
					{
						ThrowInvalidResponse(request, response, body);
					}

					return body;
				});
        }

        /// <summary>
        /// Get the response text from an IServiceFilterResponse.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>
        /// Task that will complete when the response text has been obtained.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Windows.Data.Json.JsonValue.TryParse(System.String,Windows.Data.Json.JsonValue@)", Justification = "We don't want to do anything if the Parse fails - just return null")]
        private static IJsonValue GetResponseJsonAsync(IServiceFilterResponse response)
        {
            Debug.Assert(response != null, "response cannot be null.");
            
            // Try to parse the response as JSON
            JsonValue result = null;
            if (response.Content != null)
            {
                JsonValue.TryParse(response.Content, out result);                
            }
            return result;
        }

        /// <summary>
        /// Throw an exception for an invalid response to a web request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="body">The body of the response as JSON.</param>
        private static void ThrowInvalidResponse(IServiceFilterRequest request, IServiceFilterResponse response, IJsonValue body)
        {
            Debug.Assert(request != null, "request cannot be null!");
            Debug.Assert(response != null, "response cannot be null!");
            Debug.Assert(
                response.ResponseStatus != ServiceFilterResponseStatus.Success ||
                    response.StatusCode >= 400,
                "response should be failing!");

            // Create either an invalid response or connection failed message
            // (check the status code first because some status codes will
            // set a protocol ErrorStatus).
            string message = null;
            if (response.StatusCode >= 400)
            {
                if (body != null)
                {
                    if (body.ValueType == JsonValueType.String)
                    {
                        // User scripts might return errors with just a plain string message as the
                        // body content, so use it as the exception message
                        message = body.GetString();
                    }
                    else if (body.ValueType == JsonValueType.Object)
                    {
                        // Get the error message, but default to the status description
                        // below if there's no error message present.
                        message = body.Get("error").AsString() ??
                                  body.Get("description").AsString();
                    }
                }
                
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.MobileServiceClient_ErrorMessage,
                        response.StatusDescription);
                }
            }
            else
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.MobileServiceClient_ErrorMessage,
                    response.ResponseStatus);
            }

            // Combine the pieces and throw the exception
            throw CreateMobileServiceException(message, request, response);
        }
    }
}
