using System;
using System.Dynamic;
using System.Threading.Tasks;
using ApiVersioningDemo.Middleware;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.ApiVersionMigrations
{
    public class ChangeIdToUserId : IVersionMigration
    {
        public void Up(IVersionMigrationHelper migrationHelper)
        {
            if (ShouldApply(migrationHelper.GetHttpContext()))
            {
                var user = migrationHelper.GetRequestBody<dynamic>();

                if (user.userId == null)
                {
                    user.userId = user.id;
                }

                migrationHelper.SetRequestBody(user);
            }
        }

        public void Down(IVersionMigrationHelper migrationHelper)
        {
            if (ShouldApply(migrationHelper.GetHttpContext()))
            {
                var user = migrationHelper.GetResponseBody<dynamic>();

                var oldFormat = new
                {
                    id = user.userId,
                    user.email,
                    user.name
                };

                migrationHelper.SetResponseBody(oldFormat);
            }
        }

        public Version GetVersionTag()
        {
            return Version.Parse("1.0.1");
        }
        
        private static bool ShouldApply(HttpContext context)
        {
            var request = context.Request;

            return request.Method == HttpMethods.Post && request.Path == "/user";
        }
    }
}