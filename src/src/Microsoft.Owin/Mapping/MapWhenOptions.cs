// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Owin.Mapping
{
    /// <summary>
    /// Options for the MapWhen middleware
    /// </summary>
    public partial class MapWhenOptions
    {
        /// <summary>
        /// The user callback that determines if the branch should be taken
        /// </summary>
        public Func<IOwinContext, bool> Predicate { get; set; }

        /// <summary>
        /// The branch taken for a positive match
        /// </summary>
        public OwinMiddleware Branch { get; set; }
    }
}
