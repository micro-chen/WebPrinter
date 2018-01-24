namespace System.Web.Cors
{
    using System;

    public static class CorsConstants
    {
        public static readonly string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public static readonly string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public static readonly string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public static readonly string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public static readonly string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
        public static readonly string AccessControlMaxAge = "Access-Control-Max-Age";
        public static readonly string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        public static readonly string AccessControlRequestMethod = "Access-Control-Request-Method";
        public static readonly string AnyOrigin = "*";
        public static readonly string Origin = "Origin";
        public static readonly string Referer = "Referer";
        public static readonly string Host = "Host";
        public static readonly string Token = "Token";
        public static readonly string From = "From";
        public static readonly string PreflightHttpMethod = "OPTIONS";
        internal static readonly string[] SimpleMethods = new string[] { "GET", "HEAD", "POST" };
        internal static readonly string[] SimpleRequestHeaders = new string[] { "Origin", "Accept", "Accept-Language", "Content-Language" };
        internal static readonly string[] SimpleResponseHeaders = new string[] { "Cache-Control", "Content-Language", "Content-Type", "Expires", "Last-Modified", "Pragma" };

        /// <summary>
        /// 获取带有协议+主机头+端口 网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUriAddress(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }
            var uri = new Uri(url);
            string part1 = string.Concat(uri.Scheme, "://", uri.Host);
            if (uri.Port > 0)
            {
                switch (uri.Scheme)
                {
                    case "http":
                        if (uri.Port != 80)
                        {
                            result = string.Concat(part1, ":", uri.Port);
                        }
                        break;
                    case "https":
                        if (uri.Port != 443)
                        {
                            result = string.Concat(part1, ":", uri.Port);
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}

