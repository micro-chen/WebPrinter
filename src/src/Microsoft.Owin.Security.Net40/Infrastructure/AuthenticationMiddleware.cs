// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.Owin.Security.Infrastructure
{
    public abstract class AuthenticationMiddleware<TOptions> : OwinMiddleware where TOptions : AuthenticationOptions
    {
        protected AuthenticationMiddleware(OwinMiddleware next, TOptions options)
            : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            Options = options;
        }

        public TOptions Options { get; set; }

        public override  Task Invoke(IOwinContext context)
        {
            AuthenticationHandler<TOptions> handler = CreateHandler();
            var initTask= handler.Initialize(Options, context);
            initTask.Wait();

            var handlerTask=handler.InvokeAsync();
            handlerTask.Wait();

            if (! handlerTask.Result)
            {
                 Next.Invoke(context).Wait();
            }
             handler.TeardownAsync();
        }

        protected abstract AuthenticationHandler<TOptions> CreateHandler();
    }
}
