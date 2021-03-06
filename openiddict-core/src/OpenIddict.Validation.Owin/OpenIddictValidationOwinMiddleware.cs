/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace OpenIddict.Validation.Owin
{
    /// <summary>
    /// Provides the entry point necessary to register the OpenIddict validation in an OWIN pipeline.
    /// Note: this middleware is intented to be used with dependency injection containers
    /// that support middleware resolution, like Autofac. Since it depends on scoped services,
    /// it is NOT recommended to instantiate it as a singleton like a regular OWIN middleware.
    /// </summary>
    public class OpenIddictValidationOwinMiddleware : AuthenticationMiddleware<OpenIddictValidationOwinOptions>
    {
        private readonly IOpenIddictValidationProvider _provider;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenIddictValidationOwinMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline, if applicable.</param>
        /// <param name="options">The OpenIddict validation OWIN options.</param>
        /// <param name="provider">The OpenIddict validation provider.</param>
        public OpenIddictValidationOwinMiddleware(
            [CanBeNull] OwinMiddleware next,
            [NotNull] IOptionsMonitor<OpenIddictValidationOwinOptions> options,
            [NotNull] IOpenIddictValidationProvider provider)
            : base(next, options.CurrentValue)
            => _provider = provider;

        /// <summary>
        /// Creates and returns a new <see cref="OpenIddictValidationOwinHandler"/> instance.
        /// </summary>
        /// <returns>A new instance of the <see cref="OpenIddictValidationOwinHandler"/> class.</returns>
        protected override AuthenticationHandler<OpenIddictValidationOwinOptions> CreateHandler()
            => new OpenIddictValidationOwinHandler(_provider);
    }
}
