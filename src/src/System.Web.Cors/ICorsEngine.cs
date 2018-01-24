namespace System.Web.Cors
{
    public interface ICorsEngine
    {
        CorsResult EvaluatePolicy(CorsRequestContext requestContext, CorsPolicy policy);
    }
}

