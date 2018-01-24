using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.WebApi.Owin;

namespace Owin
{
	/// <summary>
	///
	/// </summary>
	
	public static class OwinHttpMessageExtensions
	{


        private static readonly Func<OwinHttpMessageStep, Func<IDictionary<string, object>, Task>> Conversion1 =
   next => next.Invoke;

        private static readonly Func<Func<IDictionary<string, object>, Task>, OwinHttpMessageStep> Conversion2 =
            next => new OwinHttpMessageStep.CallAppFunc(next);



        private static IAppBuilder Add(IAppBuilder builder, HttpMessageInvoker invoker)
        {
            builder.AddSignatureConversion(Conversion1);
            builder.AddSignatureConversion(Conversion2);
            return builder.Use(Middleware(invoker));
        }


        /// <summary>
        /// Adds converters for adapting between disparate application signatures.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conversion"></param>
        private static void AddSignatureConversion(this IAppBuilder builder, Delegate conversion)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            object obj;
            if (builder.Properties.TryGetValue("builder.AddSignatureConversion", out obj))
            {
                var action = obj as Action<Delegate>;
                if (action != null)
                {
                    action(conversion);
                    return;
                }
            }
            throw new MissingMethodException(builder.GetType().FullName, "AddSignatureConversion");
        }

        private static Func<OwinHttpMessageStep, OwinHttpMessageStep> Middleware(HttpMessageInvoker invoker)
		{
			return (OwinHttpMessageStep next) => new OwinHttpMessageStep.CallHttpMessageInvoker(next, invoker);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		
		public static IAppBuilder UseWebApi(this IAppBuilder builder, HttpConfiguration configuration)
		{
			return OwinHttpMessageExtensions.Add(builder, new HttpMessageInvoker(new HttpServer(configuration)));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="initialize"></param>
		/// <returns></returns>
		
		public static IAppBuilder UseWebApi(this IAppBuilder builder, Action<HttpConfiguration> initialize)
		{
			if (initialize == null)
			{
				throw new ArgumentNullException("initialize");
			}
			HttpConfiguration httpConfiguration = new HttpConfiguration();
			initialize(httpConfiguration);
			return OwinHttpMessageExtensions.Add(builder, new HttpMessageInvoker(new HttpServer(httpConfiguration)));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		
		public static IAppBuilder UseWebApi(this IAppBuilder builder, HttpMessageHandler server)
		{
			return OwinHttpMessageExtensions.Add(builder, new HttpMessageInvoker(server));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <param name="dispatcher"></param>
		/// <returns></returns>
		
		public static IAppBuilder UseWebApi(this IAppBuilder builder, HttpConfiguration configuration, HttpMessageHandler dispatcher)
		{
			return OwinHttpMessageExtensions.Add(builder, new HttpMessageInvoker(new HttpServer(configuration, dispatcher)));
		}





    }
}
