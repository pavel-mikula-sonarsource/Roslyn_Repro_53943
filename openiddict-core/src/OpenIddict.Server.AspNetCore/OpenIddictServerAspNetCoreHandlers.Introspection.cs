/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace OpenIddict.Server.AspNetCore
{
    public static partial class OpenIddictServerAspNetCoreHandlers
    {
        public static class Introspection
        {
            public static ImmutableArray<OpenIddictServerHandlerDescriptor> DefaultHandlers { get; } = ImmutableArray.Create(
                /*
                 * Introspection request extraction:
                 */
                ExtractGetOrPostRequest<ExtractIntrospectionRequestContext>.Descriptor,

                /*
                 * Introspection response processing:
                 */
                ProcessJsonResponse<ApplyIntrospectionResponseContext>.Descriptor);
        }
    }
}
