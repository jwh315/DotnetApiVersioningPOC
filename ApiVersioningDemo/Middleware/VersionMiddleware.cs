using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;

namespace ApiVersioningDemo.Middleware
{
    public class VersionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly VersionMiddlewareConfigurationOptions _options;

        private static IEnumerable<IVersionMigration> _migrations;

        public VersionMiddleware(
            RequestDelegate next,
            VersionMiddlewareConfigurationOptions options
        )
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var migrationHelper = new VersionMigrationHelper(context);
            
            var applicationResponseBody = string.Empty;

            var existingBody = context.Response.Body;

            using (var newBody = new MemoryStream())
            {
                context.Response.Body = newBody;
                
                await InitializeRequestBody(context, migrationHelper);

                ProcessMigrations(migrationHelper, "up");
                
                context.Request.Body = await RewriteRequest(migrationHelper);
                
                await _next(context).ConfigureAwait(false);
                
                context.Response.Body = existingBody;

                newBody.Seek(0, SeekOrigin.Begin);

                applicationResponseBody = await new StreamReader(newBody).ReadToEndAsync();
                
                migrationHelper.SetResponseBody(applicationResponseBody);

                ProcessMigrations(migrationHelper, "down");
                
                var modifiedResponse = JsonConvert.SerializeObject(migrationHelper.GetResponseBody<object>());
                
                await context.Response.WriteAsync(modifiedResponse).ConfigureAwait(false);
            }
        }

        private static async Task<Stream> RewriteRequest(IVersionMigrationHelper migrationHelper)
        {
            var json = JsonConvert.SerializeObject(migrationHelper.GetRequestBody<object>());

            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var stream = await requestContent.ReadAsStreamAsync().ConfigureAwait(false);
            
            return stream;
        }

        private static async Task InitializeRequestBody(HttpContext context, IVersionMigrationHelper migrationHelper)
        {
            var request = context.Request;

            var requestStream = request.Body;

            var content = await new StreamReader(requestStream).ReadToEndAsync();

            migrationHelper.SetRequestBody(content);
        }

        private void ProcessMigrations(IVersionMigrationHelper migrationHelper, string direction)
        {
            Version.TryParse(_options.RequestedApiVersion(migrationHelper.GetHttpContext()), out var requestedVersion);
            
            var currentVersion = Version.Parse(_options.CurrentApiVersion);

            if (requestedVersion == null || currentVersion.Equals(requestedVersion)) return;
            
            var migrations = GetMigrations();

            if (direction == "up")
            {
                migrations = migrations.Reverse();
            }

            foreach (var migration in migrations)
            {
                if (!ShouldApply(requestedVersion, migration.GetVersionTag(), direction)) continue;
                        
                if (direction == "up")
                {
                    migration.Up(migrationHelper);
                }
                else
                {
                    migration.Down(migrationHelper);
                }
            }
        }

        private static bool ShouldApply(Version requestedVersion, Version migrationVersion, string direction)
        {
            if (direction == "up")
            {
                return migrationVersion.CompareTo(requestedVersion) >= 0;
            }

            return requestedVersion.CompareTo(migrationVersion) <= 0;
        }

        private static IEnumerable<IVersionMigration> GetMigrations()
        {
            if (_migrations == null)
            {
                var typesFromAssemblies = GetAllTypesOf<IVersionMigration>();

                _migrations = typesFromAssemblies.Select(type => (IVersionMigration) Activator.CreateInstance(type));

                _migrations.ToList().Sort((migration, versionMigration) =>
                {
                    var v1 = migration.GetVersionTag();
                    var v2 = versionMigration.GetVersionTag();

                    return v1.CompareTo(v2);
                });
            }

            return _migrations;
        }

        private static IEnumerable<Type> GetAllTypesOf<T>()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            return runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass && typeof(T).IsAssignableFrom(t));
        }
    }

    public static class VersionMiddlewareExtensions
    {
        public static IApplicationBuilder UseVersioning(this IApplicationBuilder builder,
            Action<VersionMiddlewareConfigurationOptions> options = null)
        {
            var opt = new VersionMiddlewareConfigurationOptions();
            options?.Invoke(opt);

            return builder.UseMiddleware<VersionMiddleware>(opt);
        }
    }
}