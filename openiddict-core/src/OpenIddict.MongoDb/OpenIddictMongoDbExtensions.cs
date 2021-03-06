/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes extensions allowing to register the OpenIddict MongoDB services.
    /// </summary>
    public static class OpenIddictMongoDbExtensions
    {
        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictMongoDbBuilder"/>.</returns>
        public static OpenIddictMongoDbBuilder UseMongoDb([NotNull] this OpenIddictCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Note: Mongo uses simple binary comparison checks by default so the additional
            // query filtering applied by the default OpenIddict managers can be safely disabled.
            builder.DisableAdditionalFiltering();

            builder.SetDefaultApplicationEntity<OpenIddictApplication>()
                   .SetDefaultAuthorizationEntity<OpenIddictAuthorization>()
                   .SetDefaultScopeEntity<OpenIddictScope>()
                   .SetDefaultTokenEntity<OpenIddictToken>();

            // Note: the Mongo stores/resolvers don't depend on scoped/transient services and thus
            // can be safely registered as singleton services and shared/reused across requests.
            builder.ReplaceApplicationStoreResolver<OpenIddictApplicationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceAuthorizationStoreResolver<OpenIddictAuthorizationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceScopeStoreResolver<OpenIddictScopeStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceTokenStoreResolver<OpenIddictTokenStoreResolver>(ServiceLifetime.Singleton);

            builder.Services.TryAddSingleton(typeof(OpenIddictApplicationStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictAuthorizationStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictScopeStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictTokenStore<>));

            builder.Services.TryAddSingleton<IOpenIddictMongoDbContext, OpenIddictMongoDbContext>();

            return new OpenIddictMongoDbBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the MongoDB services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public static OpenIddictCoreBuilder UseMongoDb(
            [NotNull] this OpenIddictCoreBuilder builder,
            [NotNull] Action<OpenIddictMongoDbBuilder> configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration(builder.UseMongoDb());

            return builder;
        }
    }
}
