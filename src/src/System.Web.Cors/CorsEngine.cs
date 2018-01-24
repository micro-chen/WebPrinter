namespace System.Web.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;


    public class CorsEngine : ICorsEngine
    {
        private static void AddHeaderValues(IList<string> target, IEnumerable<string> headerValues)
        {
            foreach (string str in headerValues)
            {
                target.Add(str);
            }
        }

        public virtual CorsResult EvaluatePolicy(CorsRequestContext requestContext, CorsPolicy policy)
        {
            

            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }
            CorsResult result = new CorsResult();
            if (this.TryValidateOrigin(requestContext, policy, result))
            {
                result.SupportsCredentials = policy.SupportsCredentials;
                if (requestContext.IsPreflight)
                {
                    if (this.TryValidateMethod(requestContext, policy, result))
                    {
                        if (!this.TryValidateHeaders(requestContext, policy, result))
                        {
                            return result;
                        }
                        result.PreflightMaxAge = policy.PreflightMaxAge;
                    }
                    return result;
                }
                AddHeaderValues(result.AllowedExposedHeaders, policy.ExposedHeaders);
            }
            return result;
        }

        public virtual bool TryValidateHeaders(CorsRequestContext requestContext, CorsPolicy policy, CorsResult result)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (policy.AllowAnyHeader || requestContext.AccessControlRequestHeaders.IsSubsetOf(policy.Headers))
            {
                AddHeaderValues(result.AllowedHeaders, requestContext.AccessControlRequestHeaders);
            }
            else
            {
                result.ErrorMessages.Add(string.Format(CultureInfo.CurrentCulture, "The collection of headers '{0}' is not allowed.", new object[] { string.Join(",", requestContext.AccessControlRequestHeaders) }));
            }
            return result.IsValid;
        }

        public virtual bool TryValidateMethod(CorsRequestContext requestContext, CorsPolicy policy, CorsResult result)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (policy.AllowAnyMethod || policy.Methods.Contains(requestContext.AccessControlRequestMethod))
            {
                result.AllowedMethods.Add(requestContext.AccessControlRequestMethod);
            }
            else
            {
                result.ErrorMessages.Add(string.Format(CultureInfo.CurrentCulture, "The method '{0}' is not allowed.", new object[] { requestContext.AccessControlRequestMethod }));
            }
            return result.IsValid;
        }

        public virtual bool TryValidateOrigin(CorsRequestContext requestContext, CorsPolicy policy, CorsResult result)
        {

            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (requestContext.Origin != null)
            {
                if (policy.AllowAnyOrigin)
                {
                    if (policy.SupportsCredentials)
                    {
                        result.AllowedOrigin = requestContext.Origin;
                    }
                    else
                    {
                        result.AllowedOrigin = CorsConstants.AnyOrigin;
                    }
                }
                else if (policy.Origins.Contains(requestContext.Origin))
                {
                    result.AllowedOrigin = requestContext.Origin;
                }
                else
                {
                    result.ErrorMessages.Add(string.Format(CultureInfo.CurrentCulture, "OriginNotAllowed=The origin '{0}' is not allowed.", new object[] { requestContext.Origin }));
                }
            }
            else
            {
                result.ErrorMessages.Add("The request does not contain the Origin header.");
            }
            return result.IsValid;
        }
    }
}

