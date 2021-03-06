/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Concurrent;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;

namespace OpenIddict.MongoDb
{
    /// <summary>
    /// Exposes a method allowing to resolve a token store.
    /// </summary>
    public class OpenIddictTokenStoreResolver : IOpenIddictTokenStoreResolver
    {
        private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();
        private readonly IServiceProvider _provider;

        public OpenIddictTokenStoreResolver([NotNull] IServiceProvider provider)
            => _provider = provider;

        /// <summary>
        /// Returns a token store compatible with the specified token type or throws an
        /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
        /// </summary>
        /// <typeparam name="TToken">The type of the Token entity.</typeparam>
        /// <returns>An <see cref="IOpenIddictTokenStore{TToken}"/>.</returns>
        public IOpenIddictTokenStore<TToken> Get<TToken>() where TToken : class
        {
            var store = _provider.GetService<IOpenIddictTokenStore<TToken>>();
            if (store != null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TToken), key =>
            {
                if (!typeof(OpenIddictToken).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException(new StringBuilder()
                        .AppendLine("The specified token type is not compatible with the MongoDB stores.")
                        .Append("When enabling the MongoDB stores, make sure you use the built-in 'OpenIddictToken' ")
                        .Append("entity (from the 'OpenIddict.MongoDb.Models' package) or a custom entity ")
                        .Append("that inherits from the 'OpenIddictToken' entity.")
                        .ToString());
                }

                return typeof(OpenIddictTokenStore<>).MakeGenericType(key);
            });

            return (IOpenIddictTokenStore<TToken>) _provider.GetRequiredService(type);
        }
    }
}
