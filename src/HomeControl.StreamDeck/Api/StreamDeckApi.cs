using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Client;
using RestSharp;
using Serilog;

namespace HomeControl.StreamDeck.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IStreamDeckApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>byte[]</returns>
        byte[] GetImageForKey(int? keyIndex);

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>ApiResponse of byte[]</returns>
        ApiResponse<byte[]> GetImageForKeyWithHttpInfo(int? keyIndex);
        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns></returns>
        void PressKey(int? keyIndex);

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> PressKeyWithHttpInfo(int? keyIndex);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of byte[]</returns>
        Task<byte[]> GetImageForKeyAsync(int? keyIndex);

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of ApiResponse (byte[])</returns>
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        Task<ApiResponse<byte[]>> GetImageForKeyAsyncWithHttpInfo(int? keyIndex);
        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of void</returns>
        Task PressKeyAsync(int? keyIndex);

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of ApiResponse</returns>
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        Task<ApiResponse<object>> PressKeyAsyncWithHttpInfo(int? keyIndex);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class StreamDeckApi : IStreamDeckApi
    {
        private readonly ILogger _logger;
        private ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckApi"/> class.
        /// </summary>
        /// <returns></returns>
        public StreamDeckApi(ILogger logger, String basePath)
        {
            _logger = logger;
            Configuration = new Configuration { BasePath = basePath };

            ExceptionFactory = Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public StreamDeckApi(Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
            {
                Configuration = Configuration.Default;
            }
            else
            {
                Configuration = configuration;
            }

            ExceptionFactory = Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>byte[]</returns>
        public byte[] GetImageForKey(int? keyIndex)
        {
            ApiResponse<byte[]> localVarResponse = GetImageForKeyWithHttpInfo(keyIndex);
            return localVarResponse.Data;
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>ApiResponse of byte[]</returns>
        public ApiResponse<byte[]> GetImageForKeyWithHttpInfo(int? keyIndex)
        {
            _logger.Information($"GetImageForKeyWithHttpInfo {keyIndex}");
            // verify the required parameter 'keyIndex' is set
            if (keyIndex == null)
            {
                _logger.Error($"GetImageForKeyWithHttpInfo Missing required parameter keyIndex");
                throw new ApiException(400, "Missing required parameter 'keyIndex' when calling StreamDeckApi->GetImageForKey");
            }

            var localVarPath = "/api/streamdeck/{keyIndex}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
            {
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);
            }

            if (keyIndex != null)
            {
                localVarPathParams.Add("keyIndex", Configuration.ApiClient.ParameterToString(keyIndex)); // path parameter
            }

            _logger.Information($"GetImageForKeyWithHttpInfo Making API request");

            // make the HTTP request
            var localVarResponse = (IRestResponse)Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;
            _logger.Information($"GetImageForKeyWithHttpInfo StatusCode: {localVarStatusCode}");

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetImageForKey", localVarResponse);
                if (exception != null)
                {
                    _logger.Information($"GetImageForKeyWithHttpInfo exception: {exception.Message}");
                    throw exception;
                }
            }

            _logger.Information($"GetImageForKeyWithHttpInfo returning API Response");

            return new ApiResponse<byte[]>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (byte[])Configuration.ApiClient.Deserialize(localVarResponse, typeof(byte[])));
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of byte[]</returns>
        public async Task<byte[]> GetImageForKeyAsync(int? keyIndex)
        {
            ApiResponse<byte[]> localVarResponse = await GetImageForKeyAsyncWithHttpInfo(keyIndex).ConfigureAwait(false);
            return localVarResponse.Data;

        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of ApiResponse (byte[])</returns>
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        public async Task<ApiResponse<byte[]>> GetImageForKeyAsyncWithHttpInfo(int? keyIndex)
        {
            // verify the required parameter 'keyIndex' is set
            if (keyIndex == null)
            {
                throw new ApiException(400, "Missing required parameter 'keyIndex' when calling StreamDeckApi->GetImageForKey");
            }

            var localVarPath = "/api/streamdeck/{keyIndex}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
            {
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);
            }

            if (keyIndex != null)
            {
                localVarPathParams.Add("keyIndex", Configuration.ApiClient.ParameterToString(keyIndex)); // path parameter
            }


            // make the HTTP request
            var localVarResponse = (IRestResponse)await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType).ConfigureAwait(false);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetImageForKey", localVarResponse);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return new ApiResponse<byte[]>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (byte[])Configuration.ApiClient.Deserialize(localVarResponse, typeof(byte[])));
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns></returns>
        public void PressKey(int? keyIndex)
        {
            _logger.Information($"PressKey {keyIndex}");

            PressKeyWithHttpInfo(keyIndex);
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<Object> PressKeyWithHttpInfo(int? keyIndex)
        {
            _logger.Information($"PressKeyWithHttpInfo {keyIndex}");

            // verify the required parameter 'keyIndex' is set
            if (keyIndex == null)
            {
                _logger.Error($"PressKeyWithHttpInfo Missing keyindex parameter");
                throw new ApiException(400, "Missing required parameter 'keyIndex' when calling StreamDeckApi->PressKey");
            }

            var localVarPath = "/api/streamdeck/{keyIndex}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
            {
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);
            }

            if (keyIndex != null)
            {
                localVarPathParams.Add("keyIndex", Configuration.ApiClient.ParameterToString(keyIndex)); // path parameter
            }

            // make the HTTP request
            _logger.Information($"PressKeyWithHttpInfo Making HTTP request");

            var localVarResponse = (IRestResponse)Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;
            _logger.Information($"PressKeyWithHttpInfo status code: {localVarStatusCode}");

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("PressKey", localVarResponse);
                if (exception != null)
                {
                    _logger.Information($"PressKeyWithHttpInfo exception: {exception.Message}");
                    throw exception;
                }
            }

            _logger.Information($"PressKeyWithHttpInfo returning api response");

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of void</returns>
        public async Task PressKeyAsync(int? keyIndex)
        {
            await PressKeyAsyncWithHttpInfo(keyIndex).ConfigureAwait(false);

        }

        /// <summary>
        /// </summary>
        /// <exception cref=".ApiException">Thrown when fails to make API call</exception>
        /// <param name="keyIndex"></param>
        /// <returns>Task of ApiResponse</returns>
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        public async Task<ApiResponse<object>> PressKeyAsyncWithHttpInfo(int? keyIndex)
        {
            // verify the required parameter 'keyIndex' is set
            if (keyIndex == null)
            {
                throw new ApiException(400, "Missing required parameter 'keyIndex' when calling StreamDeckApi->PressKey");
            }

            var localVarPath = "/api/streamdeck/{keyIndex}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
            {
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);
            }

            if (keyIndex != null)
            {
                localVarPathParams.Add("keyIndex", Configuration.ApiClient.ParameterToString(keyIndex)); // path parameter
            }


            // make the HTTP request
            var localVarResponse = (IRestResponse)await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType).ConfigureAwait(false);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("PressKey", localVarResponse);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
    }
}
