namespace System.Web.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class CorsResult
    {
        private long? _preflightMaxAge;

        public CorsResult()
        {
            this.AllowedMethods = new List<string>();
            this.AllowedHeaders = new List<string>();
            this.AllowedExposedHeaders = new List<string>();
            this.ErrorMessages = new List<string>();
        }

        private static void AddHeader(IDictionary<string, string> headers, string headerName, IEnumerable<string> headerValues)
        {
            string str = string.Join(",", headerValues);
            if (!string.IsNullOrEmpty(str))
            {
                headers.Add(headerName, str);
            }
        }

        public virtual IDictionary<string, string> ToResponseHeaders()
        {
            IDictionary<string, string> headers = new Dictionary<string, string>();
            if (this.AllowedOrigin != null)
            {
                headers.Add(CorsConstants.AccessControlAllowOrigin, this.AllowedOrigin);
            }
            if (this.SupportsCredentials)
            {
                headers.Add(CorsConstants.AccessControlAllowCredentials, "true");
            }
            if (this.AllowedMethods.Count > 0)
            {
                IEnumerable<string> headerValues = from m in this.AllowedMethods
                    where !CorsConstants.SimpleMethods.Contains<string>(m, StringComparer.OrdinalIgnoreCase)
                    select m;
                AddHeader(headers, CorsConstants.AccessControlAllowMethods, headerValues);
            }
            if (this.AllowedHeaders.Count > 0)
            {
                IEnumerable<string> enumerable2 = from header in this.AllowedHeaders
                    where !CorsConstants.SimpleRequestHeaders.Contains<string>(header, StringComparer.OrdinalIgnoreCase)
                    select header;
                AddHeader(headers, CorsConstants.AccessControlAllowHeaders, enumerable2);
            }
            if (this.AllowedExposedHeaders.Count > 0)
            {
                IEnumerable<string> enumerable3 = from header in this.AllowedExposedHeaders
                    where !CorsConstants.SimpleResponseHeaders.Contains<string>(header, StringComparer.OrdinalIgnoreCase)
                    select header;
                AddHeader(headers, CorsConstants.AccessControlExposeHeaders, enumerable3);
            }
            if (this.PreflightMaxAge.HasValue)
            {
                headers.Add(CorsConstants.AccessControlMaxAge, this.PreflightMaxAge.ToString());
            }
            return headers;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("IsValid: ");
            builder.Append(this.IsValid);
            builder.Append(", AllowCredentials: ");
            builder.Append(this.SupportsCredentials);
            builder.Append(", PreflightMaxAge: ");
            builder.Append(this.PreflightMaxAge.HasValue ? this.PreflightMaxAge.Value.ToString(CultureInfo.InvariantCulture) : "null");
            builder.Append(", AllowOrigin: ");
            builder.Append(this.AllowedOrigin);
            builder.Append(", AllowExposedHeaders: {");
            builder.Append(string.Join(",", this.AllowedExposedHeaders));
            builder.Append("}");
            builder.Append(", AllowHeaders: {");
            builder.Append(string.Join(",", this.AllowedHeaders));
            builder.Append("}");
            builder.Append(", AllowMethods: {");
            builder.Append(string.Join(",", this.AllowedMethods));
            builder.Append("}");
            builder.Append(", ErrorMessages: {");
            builder.Append(string.Join(",", this.ErrorMessages));
            builder.Append("}");
            return builder.ToString();
        }

        public IList<string> AllowedExposedHeaders { get; private set; }

        public IList<string> AllowedHeaders { get; private set; }

        public IList<string> AllowedMethods { get; private set; }

        public string AllowedOrigin { get; set; }

        public IList<string> ErrorMessages { get; private set; }

        public bool IsValid
        {
            get
            {
                return (this.ErrorMessages.Count == 0);
            }
        }

        public long? PreflightMaxAge
        {
            get
            {
                return this._preflightMaxAge;
            }
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", "PreflightMaxAge must be greater than or equal to 0.");
                }
                this._preflightMaxAge = value;
            }
        }

        public bool SupportsCredentials { get; set; }
    }
}

