using System;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.Middleware
{
    public class VersionMiddlewareConfigurationOptions
    {
        public Func<HttpContext, string> RequestedApiVersion { get; set; }

        public string CurrentApiVersion { get; set; }
    }
}