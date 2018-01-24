namespace System.Web.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;


    public class CorsPolicy
    {
        private long? _preflightMaxAge;

        public CorsPolicy()
        {
            this.ExposedHeaders = new List<string>();
            this.Headers = new List<string>();
            this.Methods = new List<string>();
            this.Origins = new List<string>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("AllowAnyHeader: ");
            builder.Append(this.AllowAnyHeader);
            builder.Append(", AllowAnyMethod: ");
            builder.Append(this.AllowAnyMethod);
            builder.Append(", AllowAnyOrigin: ");
            builder.Append(this.AllowAnyOrigin);
            builder.Append(", PreflightMaxAge: ");
            builder.Append(this.PreflightMaxAge.HasValue ? this.PreflightMaxAge.Value.ToString(CultureInfo.InvariantCulture) : "null");
            builder.Append(", SupportsCredentials: ");
            builder.Append(this.SupportsCredentials);
            builder.Append(", Origins: {");
            builder.Append(string.Join(",", this.Origins));
            builder.Append("}");
            builder.Append(", Methods: {");
            builder.Append(string.Join(",", this.Methods));
            builder.Append("}");
            builder.Append(", Headers: {");
            builder.Append(string.Join(",", this.Headers));
            builder.Append("}");
            builder.Append(", ExposedHeaders: {");
            builder.Append(string.Join(",", this.ExposedHeaders));
            builder.Append("}");
            return builder.ToString();
        }

        public bool AllowAnyHeader { get; set; }

        public bool AllowAnyMethod { get; set; }

        public bool AllowAnyOrigin { get; set; }

        public List<string> ExposedHeaders { get; private set; }

        public List<string> Headers { get; private set; }

        public List<string> Methods { get; private set; }

        public List<string> Origins { get; private set; }

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

