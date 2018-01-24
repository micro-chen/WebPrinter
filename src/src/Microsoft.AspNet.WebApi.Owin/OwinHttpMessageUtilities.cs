using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.WebApi.Owin
{

    using SendFileFunc = Func<string, long, long?, CancellationToken, Task>;


    internal static class OwinHttpMessageUtilities
	{


     

        private static Uri CreateRequestUri(IDictionary<string, object> environment, IDictionary<string, string[]> requestHeaders)
        {
            var requestScheme = OwinHttpMessageUtilities.Get<string>(environment, Constants.RequestSchemeKey);
            var requestPathBase = OwinHttpMessageUtilities.Get<string>(environment, Constants.RequestPathBaseKey);
            var requestPath = OwinHttpMessageUtilities.Get<string>(environment, Constants.RequestPathKey);
            var requestQueryString = OwinHttpMessageUtilities.Get<string>(environment, Constants.RequestQueryStringKey);

            // default values, in absence of a host header
            string host = "127.0.0.1";
            int port = String.Equals(requestScheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;

            // if a single host header is available
            string[] hostAndPort;
            if (requestHeaders.TryGetValue("Host", out hostAndPort) &&
                hostAndPort != null &&
                hostAndPort.Length == 1 &&
                !String.IsNullOrWhiteSpace(hostAndPort[0]))
            {
                // try to parse as "host:port" format
                int delimiterIndex = hostAndPort[0].LastIndexOf(':');
                int portValue;
                if (delimiterIndex != -1 &&
                    Int32.TryParse(hostAndPort[0].Substring(delimiterIndex + 1), out portValue))
                {
                    // use those two values
                    host = hostAndPort[0].Substring(0, delimiterIndex);
                    port = portValue;
                }
                else
                {
                    // otherwise treat as host name
                    host = hostAndPort[0];
                }
            }

            var uriBuilder = new UriBuilder(requestScheme, host, port, requestPathBase + requestPath);
            if (!String.IsNullOrEmpty(requestQueryString))
            {
                uriBuilder.Query = requestQueryString;
            }
            return uriBuilder.Uri;
        }


        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }

       
        internal static CancellationToken GetCancellationToken(IDictionary<string, object> environment)
        {
            return Get<CancellationToken>(environment, Constants.CallCancelledKey);
        }


        internal static HttpRequestMessage GetRequestMessage(IDictionary<string, object> environment)
		{
			string method = OwinHttpMessageUtilities.Get<string>(environment, "owin.RequestMethod");
			IDictionary<string, string[]> dictionary = OwinHttpMessageUtilities.Get<IDictionary<string, string[]>>(environment, "owin.RequestHeaders");
			Stream content = OwinHttpMessageUtilities.Get<Stream>(environment, "owin.RequestBody") ?? Stream.Null;
			Uri requestUri = OwinHttpMessageUtilities.CreateRequestUri(environment, dictionary);
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), requestUri)
			{
				Content = new StreamContent(content)
			};
			OwinHttpMessageUtilities.MapRequestProperties(httpRequestMessage, environment);
			foreach (KeyValuePair<string, string[]> current in dictionary)
			{
				if (!httpRequestMessage.Headers.TryAddWithoutValidation(current.Key, current.Value))
				{
					httpRequestMessage.Content.Headers.TryAddWithoutValidation(current.Key, current.Value);
				}
			}
			return httpRequestMessage;
		}


      

        // Map the OWIN environment keys to the request properties keys that WebApi expects.
        // TODO: In WebApi vNext it is probably more efficient to change WebApi to consume the environment keys directly.
        private static void MapRequestProperties(HttpRequestMessage requestMessage, IDictionary<string, object> environment)
        {
            requestMessage.Properties[Constants.RequestEnvironmentKey]
                = environment;

            // Client cert
            requestMessage.Properties[Constants.MSClientCertificateKey]
                = Get<X509Certificate2>(environment, Constants.ClientCertifiateKey);

            // IsLocal, Lazy<bool> expected.
            requestMessage.Properties[Constants.MSIsLocalKey]
                = new Lazy<bool>(() => Get<bool>(environment, Constants.IsLocalKey));

            // Remote End Point was only used by IsLocal to check for IPAddress.IsLoopback.
        }
        internal static Task SendResponseMessage(IDictionary<string, object> environment, HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            if (responseMessage == null)
            {
                throw new ArgumentNullException("responseMessage");
            }

            environment[Constants.ResponseStatusCodeKey] = responseMessage.StatusCode;
            environment[Constants.ResponseReasonPhraseKey] = responseMessage.ReasonPhrase;
            var responseHeaders = Get<IDictionary<string, string[]>>(environment, Constants.ResponseHeadersKey);
            var responseBody = Get<Stream>(environment, Constants.ResponseBodyKey);
            foreach (var kv in responseMessage.Headers)
            {
                responseHeaders[kv.Key] = kv.Value.ToArray();
            }
            if (responseMessage.Content != null)
            {
                // Trigger delayed/dynamic content-length calculations before enumerating the headers.
                responseMessage.Content.Headers.ContentLength = responseMessage.Content.Headers.ContentLength;

                foreach (var kv in responseMessage.Content.Headers)
                {
                    responseHeaders[kv.Key] = kv.Value.ToArray();
                }

                // Special case for static files
                FileContent fileContent = responseMessage.Content as FileContent;
                SendFileFunc sendFileFunc = Get<SendFileFunc>(environment, Constants.SendFileAsyncKey);
                if (fileContent != null && sendFileFunc != null)
                {
                    return fileContent.SendFileAsync(responseBody, sendFileFunc, cancellationToken);
                }

                return responseMessage.Content.CopyToAsync(responseBody);
            }
            return TaskHelpers.Completed();
        }
        


    }
}
