/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using Xunit;
using static OpenIddict.EntityFrameworkCore.OpenIddictApplicationStoreResolver;

namespace OpenIddict.EntityFrameworkCore.Tests
{
    public class OpenIddictApplicationStoreResolverTests
    {
        [Fact]
        public void Get_ReturnsCustomStoreCorrespondingToTheSpecifiedTypeWhenAvailable()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(Mock.Of<IOpenIddictApplicationStore<CustomApplication>>());

            var options = Mock.Of<IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions>>();
            var provider = services.BuildServiceProvider();
            var resolver = new OpenIddictApplicationStoreResolver(new TypeResolutionCache(), options, provider);

            // Act and assert
            Assert.NotNull(resolver.Get<CustomApplication>());
        }

        [Fact]
        public void Get_ThrowsAnExceptionForInvalidEntityType()
        {
            // Arrange
            var services = new ServiceCollection();

            var options = Mock.Of<IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions>>();
            var provider = services.BuildServiceProvider();
            var resolver = new OpenIddictApplicationStoreResolver(new TypeResolutionCache(), options, provider);

            // Act and assert
            var exception = Assert.Throws<InvalidOperationException>(() => resolver.Get<CustomApplication>());

            Assert.Equal(new StringBuilder()
                .AppendLine("The specified application type is not compatible with the Entity Framework Core stores.")
                .Append("When enabling the Entity Framework Core stores, make sure you use the built-in ")
                .Append("'OpenIddictApplication' entity (from the 'OpenIddict.EntityFrameworkCore.Models' package) ")
                .Append("or a custom entity that inherits from the generic 'OpenIddictApplication' entity.")
                .ToString(), exception.Message);
        }

        [Fact]
        public void Get_ThrowsAnExceptionWhenDbContextTypeIsNotAvailable()
        {
            // Arrange
            var services = new ServiceCollection();

            var options = Mock.Of<IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions>>(
                mock => mock.CurrentValue == new OpenIddictEntityFrameworkCoreOptions
                {
                    DbContextType = null
                });

            var provider = services.BuildServiceProvider();
            var resolver = new OpenIddictApplicationStoreResolver(new TypeResolutionCache(), options, provider);

            // Act and assert
            var exception = Assert.Throws<InvalidOperationException>(() => resolver.Get<OpenIddictApplication>());

            Assert.Equal(new StringBuilder()
                .AppendLine("No Entity Framework Core context was specified in the OpenIddict options.")
                .Append("To configure the OpenIddict Entity Framework Core stores to use a specific 'DbContext', ")
                .Append("use 'options.UseEntityFrameworkCore().UseDbContext<TContext>()'.")
                .ToString(), exception.Message);
        }

        [Fact]
        public void Get_ReturnsDefaultStoreCorrespondingToTheSpecifiedTypeWhenAvailable()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(Mock.Of<IOpenIddictApplicationStore<CustomApplication>>());
            services.AddSingleton(CreateStore());

            var options = Mock.Of<IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions>>(
                mock => mock.CurrentValue == new OpenIddictEntityFrameworkCoreOptions
                {
                    DbContextType = typeof(DbContext)
                });

            var provider = services.BuildServiceProvider();
            var resolver = new OpenIddictApplicationStoreResolver(new TypeResolutionCache(), options, provider);

            // Act and assert
            Assert.NotNull(resolver.Get<MyApplication>());
        }

        private static OpenIddictApplicationStore<MyApplication, MyAuthorization, MyToken, DbContext, long> CreateStore() 
            => new Mock<OpenIddictApplicationStore<MyApplication, MyAuthorization, MyToken, DbContext, long>>(
                Mock.Of<IMemoryCache>(),
                Mock.Of<DbContext>(),
                Mock.Of<IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions>>()).Object;

        public class CustomApplication { }

        public class MyApplication : OpenIddictApplication<long, MyAuthorization, MyToken> { }
        public class MyAuthorization : OpenIddictAuthorization<long, MyApplication, MyToken> { }
        public class MyScope : OpenIddictScope<long> { }
        public class MyToken : OpenIddictToken<long, MyApplication, MyAuthorization> { }
    }
}
