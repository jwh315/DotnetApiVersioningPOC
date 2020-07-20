using System;
using System.Threading.Tasks;
using ApiVersioningDemo.Middleware;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.ApiVersionMigrations
{
    public class ConvertUserIdToGuid : IVersionMigration
    {
        public void Up(IVersionMigrationHelper migrationHelper)
        {
            if (ShouldApply(migrationHelper.GetHttpContext()))
            {
                var user = migrationHelper.GetRequestBody<dynamic>();

                var id = user.id;

                if (id == null)
                {
                    user.id = Guid.NewGuid();
                }
                
                migrationHelper.SetRequestBody(user);
            }
        }

        public void Down(IVersionMigrationHelper migrationHelper)
        {
        }

        public Version GetVersionTag()
        {
            return Version.Parse("1.0.0");
        }

        private static bool ShouldApply(HttpContext context)
        {
            var request = context.Request;

            return request.Method == HttpMethods.Post && request.Path == "/user";
        }
    }
}