using System.Linq;
using ApiVersioningDemo.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiVersioningDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            var currentApiVersion = "1.0.3";

            app.UseVersioning(opt =>
            {
                opt.CurrentApiVersion = currentApiVersion;
                opt.RequestedApiVersion = context =>
                {
                    var header = context.Request.Headers["Accept"].FirstOrDefault();

                    if (header != null)
                    {
                        return header.Split("+").FirstOrDefault()?.Split("vnd.coxauto.v").LastOrDefault();
                    }

                    return currentApiVersion;
                };
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}