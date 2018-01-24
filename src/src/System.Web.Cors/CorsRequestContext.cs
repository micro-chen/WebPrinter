namespace System.Web.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class CorsRequestContext
    {
        public CorsRequestContext()
        {
            this.AccessControlRequestHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            this.Properties = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Origin: ");
            builder.Append(this.Origin ?? "null");
            builder.Append(", HttpMethod: ");
            builder.Append(this.HttpMethod ?? "null");
            builder.Append(", IsPreflight: ");
            builder.Append(this.IsPreflight);
            builder.Append(", Host: ");
            builder.Append(this.Host);
            builder.Append(", AccessControlRequestMethod: ");
            builder.Append(this.AccessControlRequestMethod ?? "null");
            builder.Append(", RequestUri: ");
            builder.Append(this.RequestUri);
            builder.Append(", AccessControlRequestHeaders: {");
            builder.Append(string.Join(",", this.AccessControlRequestHeaders));
            builder.Append("}");
            return builder.ToString();
        }

        public ISet<string> AccessControlRequestHeaders { get; private set; }

        public string AccessControlRequestMethod { get; set; }

        public string Host { get; set; }

        public string HttpMethod { get; set; }

        public bool IsPreflight
        {
            get
            {
                return (((this.Origin != null) && (this.AccessControlRequestMethod != null)) && string.Equals(this.HttpMethod, CorsConstants.PreflightHttpMethod, StringComparison.OrdinalIgnoreCase));
            }
        }

        public string Origin { get; set; }

        public IDictionary<string, object> Properties { get; private set; }

        public Uri RequestUri { get; set; }
    }
}

