// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Threading.Tasks;
using System.Web.Cors;

namespace Microsoft.Owin.Cors
{
    /// <summary>
    /// Contains the options used by the CorsMiddleware
    /// </summary>
    public class CorsOptions
    {
        /// <summary>
        /// A policy that allows all headers, all methods, any origin and supports credentials
        /// </summary>
        public static CorsOptions AllowAll
        {

            get
            {


                // Since we can't prevent this from being mutable, just create a new one everytime.
                return new CorsOptions
                {

                    PolicyProvider = new CorsPolicyProvider
                    {
                        //PolicyResolver = CreateNewCorsTask
                        PolicyResolver = context => TaskHelpers.FromResult(new CorsPolicy
                        {
                            AllowAnyHeader = true,
                            AllowAnyMethod = true,
                            AllowAnyOrigin = true,
                            SupportsCredentials = true
                        })


                    }
                };
            }
        }


        //public static Task<CorsPolicy> CreateNewCorsTask(IOwinRequest context) {
        //    var tsk = new Task<CorsPolicy>((x) => {
        //        return new CorsPolicy
        //        {
        //            AllowAnyHeader = true,
        //            AllowAnyMethod = true,
        //            AllowAnyOrigin = true,
        //            SupportsCredentials = true
        //        };
        //    }, null);
        //    tsk.Start();
        //    return tsk;
        //}

        //public static Task<CorsPolicy> CreateNullCorsTask(IOwinRequest context)
        //{
        //    var tsk = new Task<CorsPolicy>((x) => {
        //        return null;
        //    }, null);
        //    tsk.Start();
        //    return tsk;
        //}
        /// <summary>
        /// The cors policy to apply
        /// </summary>
        public ICorsPolicyProvider PolicyProvider { get; set; }

        /// <summary>
        /// The cors engine
        /// </summary>
        public ICorsEngine CorsEngine { get; set; }
    }
}
